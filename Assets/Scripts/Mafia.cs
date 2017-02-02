using UnityEngine;
using UnityEngine.Networking;

public class Mafia : NetworkBehaviour
{
    public float speed = 1;
    
    private bool startInputOnObject;

    public GameObject UIAttack;
    public GameObject shopWhereIAm;
    private bool isMine;
    public int type;

    public void MarkAsMine()
    {
        isMine = true;
        transform.GetComponent<Renderer>().material.color = Color.green;
    }
    
    void Update()
    {        
        if (isServer && shopWhereIAm != null)
        {
            PlayerManager.GetInstance().OnMafiaXPGiven(gameObject, (int)(shopWhereIAm.GetComponent<Shop>().xpBonus * Time.deltaTime * 1000));
            PoliceController.GetInstance().ShopOccupied(shopWhereIAm);
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
}

