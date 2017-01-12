using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectManager : NetworkBehaviour
{
    private class Combat
    {
        private PlayerController m_attacker;
        private PlayerController m_defender;

        private float m_attackerDelay;
        private float m_defenderDelay;

        public Combat(PlayerController attacker, PlayerController defender)
        {
            m_attacker = attacker;
            m_defender = defender;
        }

        public bool Update()
        {
            m_attackerDelay += Time.deltaTime;
            m_defenderDelay += Time.deltaTime;

            bool end = false;

            while (!end && m_attacker.attackSpeed > m_attackerDelay)
            {
                end = Hit(m_defender, m_attacker);
                m_attackerDelay -= m_attacker.attackSpeed;
            }
            while (!end && m_defender.attackSpeed > m_defenderDelay)
            {
                end = Hit(m_attacker, m_defender);
                m_defenderDelay -= m_defender.attackSpeed;
            }

            return end;
        }
    }

    private static bool Hit(PlayerController defender, PlayerController attacker)
    {
        defender.currentHealth -= attacker.attackPower;
        if (defender.currentHealth > 0)
        {
            defender.healthBar.fillAmount = (float)defender.currentHealth / defender.maxHealth;
            return true;
        }
        if (defender.shopWhereIAm != null)
        {
            defender.GetComponent<Shop>().OnSettleDead();
        }
        Destroy(defender);
        return false;
    }

    private static ArrayList combats = new ArrayList();

    public void StartCombat(NetworkInstanceId attackerID, NetworkInstanceId defenderID)
    {
        CmdStartCombat(attackerID, defenderID);
    }

    [Command]
    public void CmdStartCombat(NetworkInstanceId attackerID, NetworkInstanceId defenderID)
    {
        combats.Add(new Combat(NetworkServer.FindLocalObject(attackerID).GetComponent<PlayerController>(),
                                NetworkServer.FindLocalObject(defenderID).GetComponent<PlayerController>()));
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
        settledPlayer.GetComponent<PlayerController>().currentHealth = 5000;
        settledPlayer.GetComponent<PlayerController>().OnChangeHealth(5000);
        Physics.IgnoreCollision(settledPlayer.GetComponent<Collider>(), shop.GetComponent<Collider>());
        settledPlayer.GetComponent<Transform>().position = shop.GetComponent<Transform>().position;
        settledPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        settledPlayer.GetComponent<Rigidbody>().isKinematic = false;
        settledPlayer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
    }

    public static ObjectManager GetInstance()
    {
        return GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
    }
}
