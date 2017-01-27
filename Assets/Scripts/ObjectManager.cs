using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectManager : NetworkBehaviour
{
    private class Combat
    {
        private CombatUnit m_attacker;
        private CombatUnit m_defender;

        private float m_attackerDelay;
        private float m_defenderDelay;

        public Combat(CombatUnit attacker, CombatUnit defender)
        {
            m_attackerDelay = 0;
            m_defenderDelay = 0;
            m_attacker = attacker;
            m_defender = defender;
        }

        public bool Update()
        {
            if (m_attacker == null || m_defender == null)
                return true;

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

    private static bool Hit(CombatUnit defender, CombatUnit attacker)
    {
        int damage = (int)(defender.GetShopDefenceBonus() * attacker.attackPower);
        PlayerManager.GetInstance().OnMafiaDamageGiven(attacker.gameObject, damage);
        defender.currentHealth -= damage;
        defender.OnChangeHealth(defender.currentHealth);
        if (defender.currentHealth > 0)
            return false;
        PlayerController.GetInstance().RpcMafiaDead(defender.gameObject.GetComponent<NetworkIdentity>().netId);
        defender.Dead();
        NetworkServer.Destroy(defender.gameObject);
        return true;
    }

    private static ArrayList combats = new ArrayList();

    public void StartCombat(NetworkInstanceId attackerID, NetworkInstanceId defenderID)
    {
        combats.Add(new Combat(NetworkServer.FindLocalObject(attackerID).GetComponent<CombatUnit>(),
                                NetworkServer.FindLocalObject(defenderID).GetComponent<CombatUnit>()));
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
        Physics.IgnoreCollision(settledPlayer.GetComponent<Collider>(), shop.GetComponent<Collider>());
        settledPlayer.GetComponent<Transform>().position = shop.GetComponent<Transform>().position;
        settledPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        settledPlayer.GetComponent<Rigidbody>().isKinematic = false;
        settledPlayer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        settledPlayer.GetComponent<Mafia>().shopWhereIAm = shop;
        settledPlayer.GetComponent<CombatUnit>().shopWhereIAm = shop;
        shop.GetComponent<Shop>().settledPlayer = settledPlayer;
        shop.GetComponent<Shop>().settledPlayerNetId = settledPlayer.GetComponent<NetworkIdentity>().netId;
        PlayerController.GetInstance().RpcSettlePlayerInShop(settledPlayerID, shopID);
    }

    public static ObjectManager GetInstance()
    {
        return GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
    }
}
