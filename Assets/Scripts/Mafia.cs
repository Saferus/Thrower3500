using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Mafia : NetworkBehaviour
{
    public float speed = 1;
    public GameObject trajectory;
    public GameObject trajectoryInstance;
    public float trajectoryScale;
    private float trajectoryAngle = 0;

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
    public bool isMine;
    public int type;

    // Use this for initialization
    void Start()
    {
        healthBar = transform.FindChild("HealthBar").FindChild("Health").GetComponent<Image>();
        rb = GetComponent<Rigidbody>();
        healthBar.fillAmount = ((float)currentHealth) / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && shopWhereIAm != null)
        {
            PlayerManager.GetInstance().OnMafiaXPGiven(gameObject, (int) (shopWhereIAm.GetComponent<Shop>().xpBonus * Time.deltaTime * 1000));
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
        if (isMine)
        {
            startPos = new Vector2(pos.x, pos.y);
            CreateTrajectory();
            FocusManager.SetFocusedPlayer(gameObject);
            Camera.main.GetComponent<CameraControl>().isFixed = true;
        }
    }

    private void OnDrag(Vector2 pos)
    {
        if (isMine)
            RescaleTrajectory(pos);
    }

    private void OnRelease(Vector2 pos)
    {
        if (isMine)
        {
            PlayerController.GetLocalInstance().AddForce(GetComponent<NetworkIdentity>().netId, startPos, pos);
            startInputOnObject = false;
            DeleteTrajectory();
            Camera.main.GetComponent<CameraControl>().isFixed = false;
        }
        else
        {
            if (FocusManager.GetCurrentFocusedPlayer() != null)
            {

            }
        }
    }

    private void CreateTrajectory()
    {
        Vector3 pos = new Vector3(rb.position.x, rb.position.y, rb.position.z);
        Quaternion rot = Quaternion.AngleAxis(Mathf.Cos(pos.x / (Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y + pos.z * pos.z))), Vector3.left);
        trajectoryInstance = (GameObject)Instantiate(trajectory, pos, rot);
        RescaleTrajectory(pos);
    }

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
        trajectoryAngle = 0;
        Destroy(trajectoryInstance);
    }

    private void OnMouseOver()
    {
        if (shopWhereIAm != null)
        {
            return;
        }
        startInputOnObject = true;
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
    
    public void AddForce(Vector2 startPos, Vector2 pos)
    {
        rb.AddForce(new Vector3(startPos.y - pos.y, 0, pos.x - startPos.x) * speed);
        RpcAddClientForce(startPos, pos);
    }

    [ClientRpc]
    public void RpcAddClientForce(Vector2 startPos, Vector2 pos)
    {
        rb.AddForce(new Vector3(startPos.y - pos.y, 0, pos.x - startPos.x) * speed);
    }
    
    public void Dead()
    {
        if (shopWhereIAm != null)
        {
            shopWhereIAm.GetComponent<Shop>().OnSettleDead();
        }
    }

    public float GetShopDefenceBonus()
    {
        if (shopWhereIAm == null)
            return 1;
        return shopWhereIAm.GetComponent<Shop>().deffenceBonus;
    }
}

