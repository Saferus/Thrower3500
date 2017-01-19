using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    class Player
    {
        private static int s_playerCounter = 0;
        public string m_playerName { get; private set; }
        public int m_xpCount;
        public int m_spawnCounter;

        public Player()
        {
            m_playerName = "Player" + s_playerCounter;
            m_xpCount = 0;
            m_spawnCounter = 0;
            s_playerCounter++;
        }

        public string GetInfo()
        {
            return m_playerName + " : xp=" + m_xpCount + "; spawnCounter=" + m_spawnCounter;
        }
    }

    private Dictionary<string, Player> connectedPlayers = new Dictionary<string, Player>();
    public GameObject mafiaPrefab1;
    public GameObject mafiaPrefab2;
    public GameObject mafiaPrefab3;

    public float damageToExpCoef = 2;

    public void OnSpawnClicked(string deviceID, int mafiaId)
    {
        GameObject[] mafiaOfThisPlayer = GameObject.FindGameObjectsWithTag(connectedPlayers[deviceID].m_playerName);
        foreach (GameObject mafiaByType in mafiaOfThisPlayer)
        {
            if (mafiaByType.GetComponent<Mafia>().type == mafiaId)
                return;
        }
        GameObject mafia = SpawnMafia(connectedPlayers[deviceID], mafiaId);
        NetworkInstanceId[] playersMafiaIDs = new NetworkInstanceId[1];
        playersMafiaIDs[0] = mafia.GetComponent<NetworkIdentity>().netId;
        RpcAssignMafia(deviceID, playersMafiaIDs);
    }

    public void PlayerConnected(string deviceID)
    {
        if (!connectedPlayers.ContainsKey(deviceID))
        {
            Player player = new Player();
            connectedPlayers.Add(deviceID, player);
        }
        GameObject[] playersMafia = GameObject.FindGameObjectsWithTag(connectedPlayers[deviceID].m_playerName);
        NetworkInstanceId[] playersMafiaIDs = new NetworkInstanceId[playersMafia.Length];

        for (int i = 0; i < playersMafia.Length; i++)
        {
            playersMafiaIDs[i] = playersMafia[i].GetComponent<NetworkIdentity>().netId;
        }

        RpcAssignMafia(deviceID, playersMafiaIDs);
    }

    private GameObject SpawnMafia(Player player, int mafiaId)
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        GameObject mafiaPrefab = null;

        switch (mafiaId)
        {
            case 1:
                mafiaPrefab = mafiaPrefab1;
                break;
            case 2:
                mafiaPrefab = mafiaPrefab2;
                break;
            case 3:
                mafiaPrefab = mafiaPrefab3;
                break;
        }

        GameObject mafiaObj = Instantiate(mafiaPrefab,
            spawnPoints[(int) Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
        mafiaObj.gameObject.tag = player.m_playerName;
        player.m_spawnCounter++;
        NetworkServer.Spawn(mafiaObj);
        mafiaObj.GetComponent<Mafia>().type = mafiaId;
        return mafiaObj;
    }

    [ClientRpc]
    public void RpcAssignMafia(string deviceID, NetworkInstanceId[] playersMafiaIDs)
    {
        if (deviceID == SystemInfo.deviceUniqueIdentifier)
        {
            AssignAllMafiaOfType("Mafia1", playersMafiaIDs);
            AssignAllMafiaOfType("Mafia2", playersMafiaIDs);
            AssignAllMafiaOfType("Mafia3", playersMafiaIDs);
        }
    }

    private void AssignAllMafiaOfType(string tag, NetworkInstanceId[] playersMafiaIDs)
    {
        GameObject[] allMafia = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject mafia in allMafia)
        {
            foreach (NetworkInstanceId netID in playersMafiaIDs)
            {
                if (netID == mafia.GetComponent<NetworkIdentity>().netId)
                {
                    mafia.GetComponent<Mafia>().isMine = true;
                }
            }
        }
    }

    public static PlayerManager GetInstance()
    {
        return GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }

    public string GetPlayersInfo()
    {
        string info = "";
        foreach (Player player in connectedPlayers.Values)
        {
            info += player.GetInfo() + "\n";
        }
        return info;
    }

    public void OnMafiaDamageGiven(GameObject mafia, int damage)
    {
        OnMafiaXPGiven(mafia, (int) (damage * damageToExpCoef));
    }

    public void OnMafiaXPGiven(GameObject mafia, int damage)
    {
        foreach (Player player in connectedPlayers.Values.ToList())
        {
            if (mafia.tag == player.m_playerName)
            {
                player.m_xpCount += damage;
                return;
            }
        }
    }
}
