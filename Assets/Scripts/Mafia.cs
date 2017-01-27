using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Mafia : NetworkBehaviour
{
    public float speed = 1;

    private Rigidbody rb;
    private bool startInputOnObject;

    public int attackPower = 50;
    public float attackSpeed;
    public int maxHealth = 20000;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = 20000;
    public Image healthBar;

    public GameObject UIAttack;
    public GameObject shopWhereIAm;
    private bool isMine;
    public int type;

    public void MarkAsMine()
    {
        isMine = true;
        transform.GetComponent<Renderer>().material.color = Color.green;
    }
    
    void Start()
    {
        healthBar = transform.FindChild("HealthBar").FindChild("Health").GetComponent<Image>();
        rb = GetComponent<Rigidbody>();
        healthBar.fillAmount = ((float) currentHealth) / maxHealth;
    }
    
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
            FocusManager.SetFocusedPlayer(gameObject);
            Camera.main.GetComponent<CameraControl>().isFixed = true;
        }
    }

    private void OnDrag(Vector2 pos) { }

    private void OnRelease(Vector2 pos)
    {
        startInputOnObject = false;
        if (isMine)
        {
            Camera.main.GetComponent<CameraControl>().isFixed = false;
        }
        else
        {
            if (FocusManager.GetCurrentFocusedPlayer() != null)
            {
                FocusManager.SetFocusedEnemy(gameObject);
                UIAttack.GetComponent<AttackUIController>().ShowOnEnemy();
            }
        }
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

