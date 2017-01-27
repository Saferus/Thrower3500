using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CombatUnit : NetworkBehaviour {

    public int attackPower = 50;
    public float attackSpeed;
    public int maxHealth = 20000;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = 20000;
    public Image healthBar;
    private Shop shopWhereIAm;

    void Start()
    {
        healthBar = transform.FindChild("HealthBar").FindChild("Health").GetComponent<Image>();
        healthBar.fillAmount = ((float)currentHealth) / maxHealth;

        Mafia mafia = gameObject.GetComponent<Mafia>();
        if (mafia != null && mafia.shopWhereIAm != null)
            shopWhereIAm = mafia.shopWhereIAm.GetComponent<Shop>();
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
}
