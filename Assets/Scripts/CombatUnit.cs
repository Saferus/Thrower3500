using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CombatUnit : NetworkBehaviour {

    public int attackPower = 50;
    public float attackSpeed;
    public int maxHealth = 20000;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = 20000;
    private Image healthBar;
    public GameObject shopWhereIAm { get; set; }

    void Start()
    {
        healthBar = transform.FindChild("HealthBar").FindChild("Health").GetComponent<Image>();
        healthBar.fillAmount = ((float)currentHealth) / maxHealth;
    }

    public void OnChangeHealth(int currentHealth)
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    public float GetShopDefenceBonus()
    {
        if (shopWhereIAm == null)
            return 1;
        return shopWhereIAm.GetComponent<Shop>().deffenceBonus;
    }

    public void Dead()
    {
        if (shopWhereIAm != null)
        {
            shopWhereIAm.GetComponent<Shop>().OnSettleDead();
        }
    }

    public void OnCombatWin()
    {
        if (gameObject.tag == "PoliceUnit")
        {
            Destroy(gameObject);
        }
    }
}
