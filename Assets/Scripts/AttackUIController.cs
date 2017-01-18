using UnityEngine;
using UnityEngine.UI;

public class AttackUIController : MonoBehaviour
{
    private const int TYPE_NONE = 0;
    private const int TYPE_ON_SHOP = TYPE_NONE + 1;
    private const int TYPE_ON_ENEMY = TYPE_ON_SHOP + 1;

    public GameObject UIAttackPrefab;
    private int type;

    private void Instantiate()
    {
        Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);
        Instantiate(UIAttackPrefab, Vector3.zero, rot);
        gameObject.transform.Find("Attack").GetComponent<Button>().onClick.AddListener(() => OnAttack());
        gameObject.transform.Find("Settle").GetComponent<Button>().onClick.AddListener(() => OnSettle());
    }

    public void ShowOnShop()
    {
        if (FocusManager.GetCurrentFocusedPlayer() != null && FocusManager.GetCurrentFocusedBuilding() != null)
        {
            Instantiate();
            type = TYPE_ON_SHOP;
        }
    }

    public void ShowOnEnemy()
    {
        if (FocusManager.GetCurrentFocusedPlayer() != null && FocusManager.GetCurrentFocusedEnemy() != null)
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

    public static void HideUI()
    {
        Destroy(GameObject.Find("AttackUI"));
    }
}
