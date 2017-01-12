using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public float speed = 1;
    public GameObject trajectory;
    public GameObject trajectoryInstance;
    public float trajectoryScale;

    private Rigidbody rb;
    private bool startInputOnObject;
    private Vector2 startPos;
    private Vector2 force;
    
    public int attackPower = 50;
    public float attackSpeed;
    public int maxHealth = 20000;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = 20000;
    public Image healthBar;

    public GameObject shopWhereIAm;

    // Use this for initialization
    void Start ()
    {
        healthBar = transform.FindChild("HealthBar").FindChild("Health").GetComponent<Image>();
        rb = GetComponent<Rigidbody>();
        healthBar.fillAmount = ((float) currentHealth) / maxHealth;
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (startInputOnObject)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            OnPress(touch.position);
                            break;
                        case TouchPhase.Moved:
                            OnDrag(touch.position);
                            break;
                        case TouchPhase.Ended:
                            OnRelease(touch.position);
                            break;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnPress(Input.mousePosition);
                    return;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    OnRelease(Input.mousePosition);
                    return;
                }
                if (Input.GetMouseButton(0))
                {
                    OnDrag(Input.mousePosition);
                    return;
                }
            }
        }
    }

    private void OnPress(Vector2 pos)
    {
        startPos = new Vector2(pos.x, pos.y);
        CreateTrajectory();
        FocusManager.SetFocusedPlayer(gameObject);
    }

    private void OnDrag(Vector2 pos)
    {
        RescaleTrajectory(pos);
    }

    private void OnRelease(Vector2 pos)
    {
        CmdAddForce(startPos, pos);
        startInputOnObject = false;
        DeleteTrajectory();
    }

    private void CreateTrajectory()
    {
        Vector3 pos = new Vector3(rb.position.x, rb.position.y, rb.position.z);
        Quaternion rot = Quaternion.AngleAxis(Mathf.Cos(pos.x / (Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y + pos.z * pos.z))), Vector3.left);
        trajectoryInstance = (GameObject) Instantiate(trajectory, pos, rot);
        RescaleTrajectory(pos);
    }

    private static float TRAJECTORY_START_ANGLE = Mathf.PI / 2;
    private float trajectoryAngle = TRAJECTORY_START_ANGLE;

    private void RescaleTrajectory(Vector3 pos)
    {
        float delta = Mathf.Sqrt(Mathf.Pow((pos.x - startPos.x), 2) + Mathf.Pow((pos.y - startPos.y), 2));
        float angle = 0;
        if (delta > 0)
            angle = Mathf.Acos((pos.y - startPos.y) / delta);
        if (pos.x < startPos.x)
            angle *= -1;
        trajectoryInstance.transform.localScale = new Vector3(delta * trajectoryScale, trajectoryScale, trajectoryScale);
        trajectoryInstance.transform.Rotate(Vector3.down * (trajectoryAngle - angle) * 180 / Mathf.PI);
        trajectoryAngle = angle;
    }

    private void DeleteTrajectory()
    {
        trajectoryAngle = TRAJECTORY_START_ANGLE;
        Destroy(trajectoryInstance);
    }

    private void OnMouseOver()
    {
        if (!isLocalPlayer || shopWhereIAm != null)
        {
            return;
        }
        startInputOnObject = true;
    }

    public void SetshopWhereIAm(GameObject shop)
    {
        shopWhereIAm = shop;
    }

    public void OnChangeHealth(int currentHealth)
    {
        if (currentHealth < 0)
        {
            if (shopWhereIAm != null)
                shopWhereIAm.GetComponent<Shop>().OnSettleDead();
        }
        else
            healthBar.fillAmount = (float)currentHealth / maxHealth;
    }
    
    public void OnAttackClicked()
    {
        Shop shop = FocusManager.GetCurrentFocusedBuilding().GetComponent<Shop>();
        if (shop.settledPlayer != null)
            CmdOnAttackClicked(FocusManager.GetCurrentFocusedPlayer().GetComponent<NetworkIdentity>().netId,
                shop.settledPlayer.GetComponent<NetworkIdentity>().netId);
        FocusManager.SetFocusedPlayer(null);
        FocusManager.SetFocusedBuilding(null);
        shop.HideUI();
    }

    [Command]
    public void CmdOnAttackClicked(NetworkInstanceId attackPlayerID, NetworkInstanceId defencePlayerID)
    {
        ObjectManager.GetInstance().StartCombat(attackPlayerID, defencePlayerID);
    }

    public void OnSettleClicked()
    {
        Shop shop = FocusManager.GetCurrentFocusedBuilding().GetComponent<Shop>();
        if (shop.settledPlayer == null)
        {
            CmdOnSettleClicked(FocusManager.GetCurrentFocusedBuilding().GetComponent<NetworkIdentity>().netId,
                                FocusManager.GetCurrentFocusedPlayer().GetComponent<NetworkIdentity>().netId);
        }
        FocusManager.SetFocusedPlayer(null);
        FocusManager.SetFocusedBuilding(null);
        shop.HideUI();
    }

    [Command]
    public void CmdOnSettleClicked(NetworkInstanceId shopID, NetworkInstanceId settledPlayerID)
    {
        ObjectManager.GetInstance().SettlePlayerInShop(settledPlayerID, shopID);
    }

    [Command]
    public void CmdAddForce(Vector2 startPos, Vector2 pos)
    {
        RpcAddClientForce(startPos, pos);
    }

    [ClientRpc]
    public void RpcAddClientForce(Vector2 startPos, Vector2 pos)
    {
        rb.AddForce(new Vector3(startPos.x - pos.x, 0, startPos.y - pos.y) * speed);
    }

    [ClientRpc]
    public void RpcDead()
    {
        if (shopWhereIAm != null)
        {
            GetComponent<Shop>().OnSettleDead();
        }
        Destroy(gameObject);
    }
}
