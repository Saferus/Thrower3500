using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    private Dictionary<string, string> connectedPlayers = new Dictionary<string, string>();
    private int dictionaryLenght = 0;
    public GameObject mafiaPrefab;

    public void PlayerConnected(string deviceID)
    {
        if (!connectedPlayers.ContainsKey(deviceID))
        {
            string player = "Player" + dictionaryLenght;
            connectedPlayers.Add(deviceID, player);
            dictionaryLenght++;
            GameObject mafiaObj = (GameObject)Instantiate(mafiaPrefab, Vector3.zero, Quaternion.identity);
            mafiaObj.gameObject.tag = player;
            NetworkServer.Spawn(mafiaObj);
        }
        GameObject[] playersMafia = GameObject.FindGameObjectsWithTag(connectedPlayers[deviceID]);
        NetworkInstanceId[] playersMafiaIDs = new NetworkInstanceId[playersMafia.Length];
        for( int i = 0; i < playersMafia.Length; i++)
        {
            playersMafiaIDs[i] = playersMafia[i].GetComponent<NetworkIdentity>().netId;
        }

        RpcAssignMafia(deviceID, playersMafiaIDs);
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
