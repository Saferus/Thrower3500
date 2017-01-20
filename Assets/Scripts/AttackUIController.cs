using UnityEngine;
using UnityEngine.UI;

public class AttackUIController : MonoBehaviour
{
    private const int TYPE_NONE = 0;
    private const int TYPE_ON_SHOP = TYPE_NONE + 1;
    private const int TYPE_ON_ENEMY = TYPE_ON_SHOP + 1;

    public GameObject UIAttackPrefab;
    static public GameObject UIAttackInstance;
    private int type;

    private void Instantiate()
    {
        Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);
        UIAttackInstance = Instantiate(UIAttackPrefab, Vector3.zero, rot);
        GameObject.Find("Attack").GetComponent<Button>().onClick.AddListener(() => OnAttack());
        GameObject.Find("Settle").GetComponent<Button>().onClick.AddListener(() => OnSettle());
        GameObject.Find("Cancel").GetComponent<Button>().onClick.AddListener(() => OnCancel());
    }

    public void ShowOnShop()
    {
        if (FocusManager.GetCurrentFocusedPlayer() != null && FocusManager.GetCurrentFocusedBuilding() != null && UIAttackInstance == null)
        {
            Instantiate();
            type = TYPE_ON_SHOP;

            // hide unused button
            Shop shop = FocusManager.GetCurrentFocusedBuilding().GetComponent<Shop>();
            if (shop.settledPlayer == null)
            {
                Destroy(GameObject.Find("Attack"));
                Vector3 posAttack = GameObject.Find("Attack").transform.position;
                GameObject.Find("Settle").transform.position = posAttack;
            }
            else
            {
                Destroy(GameObject.Find("Settle"));
            }
        }
    }

    public void ShowOnEnemy()
    {
        if (FocusManager.GetCurrentFocusedPlayer() != null && FocusManager.GetCurrentFocusedEnemy() != null && UIAttackInstance == null)
        {
            Instantiate();
            type = TYPE_ON_ENEMY;
        }
    }

    public void OnAttack()
    {
        HideUI();
        if (type == TYPE_ON_SHOP)
        {
            Shop.OnAttackClicked();
        }
    }

    public void OnSettle()
    {
        HideUI();
        if (type == TYPE_ON_SHOP)
        {
            Shop.OnSettleClicked();
        }
    }

    public void OnCancel()
    {
        HideUI();
        type = TYPE_NONE;
    }

    public void HideUI()
    {
        Destroy(UIAttackInstance);
    }
}
