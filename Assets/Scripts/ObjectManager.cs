using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectManager : NetworkBehaviour
{
    private class Combat
    {
        private Mafia m_attacker;
        private Mafia m_defender;

        private float m_attackerDelay;
        private float m_defenderDelay;

        public Combat(Mafia attacker, Mafia defender)
        {
            m_attackerDelay = 0;
            m_defenderDelay = 0;
            m_attacker = attacker;
            m_defender = defender;
        }

        public bool Update()
        {
            m_attackerDelay += Time.deltaTime * 1000;
            m_defenderDelay += Time.deltaTime * 1000;

            bool end = false;

            while (!end && m_attacker.attackSpeed < m_attackerDelay)
            {
                end = Hit(m_defender, m_attacker);
                m_attackerDelay -= m_attacker.attackSpeed;
            }
            while (!end && m_defender.attackSpeed < m_defenderDelay)
            {
                end = Hit(m_attacker, m_defender);
                m_defenderDelay -= m_defender.attackSpeed;
            }

            return end;
        }
    }

    private static bool Hit(Mafia defender, Mafia attacker)
    {
        defender.currentHealth -= attacker.attackPower;
        if (defender.currentHealth > 0)
        {
            defender.healthBar.fillAmount = (float)defender.currentHealth / defender.maxHealth;
            return false;
        }
        defender.RpcDead();
        if (defender.shopWhereIAm != null)
        {
            defender.GetComponent<Shop>().OnSettleDead();
        }
        Destroy(defender.gameObject);
        return true;
    }

    private static ArrayList combats = new ArrayList();

    public void StartCombat(NetworkInstanceId attackerID, NetworkInstanceId defenderID)
    {
        combats.Add(new Combat(NetworkServer.FindLocalObject(attackerID).GetComponent<Mafia>(),
                                NetworkServer.FindLocalObject(defenderID).GetComponent<Mafia>()));
    }
	
	void Update ()
    {
        for (int i = 0; i < combats.Count; i++)
        {
            bool end = ((Combat) combats[i]).Update();
            if (end)
                combats.RemoveAt(i);
        }
    }

    public void SettlePlayerInShop(NetworkInstanceId settledPlayerID, NetworkInstanceId shopID)
    {
        GameObject shop = NetworkServer.FindLocalObject(shopID);
        GameObject settledPlayer = NetworkServer.FindLocalObject(settledPlayerID);
        settledPlayer.GetComponent<Mafia>().currentHealth = 15000;
        settledPlayer.GetComponent<Mafia>().OnChangeHealth(15000);
        Physics.IgnoreCollision(settledPlayer.GetComponent<Collider>(), shop.GetComponent<Collider>());
        settledPlayer.GetComponent<Transform>().position = shop.GetComponent<Transform>().position;
        settledPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        settledPlayer.GetComponent<Rigidbody>().isKinematic = false;
        settledPlayer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        shop.GetComponent<Shop>().settledPlayer = settledPlayer;
        RpcSettlePlayerInShop(settledPlayerID, shopID);
    }

    [ClientRpc]
    public void RpcSettlePlayerInShop(NetworkInstanceId settledPlayerID, NetworkInstanceId shopID)
    {
        GameObject shop = ClientScene.FindLocalObject(shopID);
        GameObject settledPlayer = ClientScene.FindLocalObject(settledPlayerID);
        Physics.IgnoreCollision(settledPlayer.GetComponent<Collider>(), shop.GetComponent<Collider>());
        settledPlayer.GetComponent<Transform>().position = shop.GetComponent<Transform>().position;
        settledPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        settledPlayer.GetComponent<Rigidbody>().isKinematic = false;
        settledPlayer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        shop.GetComponent<Shop>().settledPlayer = settledPlayer;
        settledPlayer.GetComponent<Mafia>().shopWhereIAm = shop;
    }

    public static ObjectManager GetInstance()
    {
        return GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
    }
}
