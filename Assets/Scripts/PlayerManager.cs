using UnityEngine;
using System.Collections.Generic;
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
    }

    private Dictionary<string, Player> connectedPlayers = new Dictionary<string, Player>();
    public GameObject mafiaPrefab1;
    public GameObject mafiaPrefab2;
    public GameObject mafiaPrefab3;

    public void OnSpawnClicked(string deviceID, int mafiaId)
    {

        GameObject mafia =  SpawnMafia(connectedPlayers[deviceID]);
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

        for ( int i = 0; i < playersMafia.Length; i++)
        {
            playersMafiaIDs[i] = playersMafia[i].GetComponent<NetworkIdentity>().netId;
        }

        RpcAssignMafia(deviceID, playersMafiaIDs);
    }

    private GameObject SpawnMafia(Player player)
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        GameObject mafiaObj = (GameObject)Instantiate(mafiaPrefab1, spawnPoints[(int) Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
        mafiaObj.gameObject.tag = player.m_playerName;
        player.m_spawnCounter++;
        NetworkServer.Spawn(mafiaObj);
        return mafiaObj;
    }

    [ClientRpc]
    public void RpcAssignMafia(string deviceID, NetworkInstanceId[] playersMafiaIDs)
    {
        if (deviceID == SystemInfo.deviceUniqueIdentifier)
        {
            GameObject[] allMafia = GameObject.FindGameObjectsWithTag("Mafia");
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
    }

    public static PlayerManager GetInstance()
    {
        return GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }
}
