using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public GameObject mafiaPrefab1;
    public GameObject mafiaPrefab2;
    public GameObject mafiaPrefab3;
    public GameObject playerHUD;

    private static PlayerController localController;

    public static PlayerController GetLocalInstance()
    {
        return localController;
    }

    public override void OnStartLocalPlayer()
    {
        CmdPlayerConnected(SystemInfo.deviceUniqueIdentifier);
        localController = this;
        GameObject[] shops = GameObject.FindGameObjectsWithTag("Shop");
        foreach (GameObject shop in shops)
        {
            shop.GetComponent<Shop>().SynchronizeData();
        }
        Instantiate(playerHUD, Vector3.zero, Quaternion.identity);
        GameObject.Find("SpawnMafia1").GetComponent<Button>().onClick.AddListener(() => OnSpawnClicked(1));
        GameObject.Find("SpawnMafia2").GetComponent<Button>().onClick.AddListener(() => OnSpawnClicked(2));
        GameObject.Find("SpawnMafia3").GetComponent<Button>().onClick.AddListener(() => OnSpawnClicked(3));
    }
    
    public void OnSpawnClicked(int mafiaId)
    {
        CmdOnSpawnClicked(SystemInfo.deviceUniqueIdentifier, mafiaId);
    }

    [Command]
    public void CmdOnSpawnClicked(string deviceID, int mafiaId)
    {
        PlayerManager.GetInstance().OnSpawnClicked(deviceID, mafiaId);
    }

    [Command]
    public void CmdPlayerConnected(string deviceID)
    {
        PlayerManager.GetInstance().PlayerConnected(deviceID);
    }

    public void SpawnMafia(string deviceID)
    {
        GameObject mafiaObj = (GameObject) Instantiate(mafiaPrefab1, Vector3.zero, Quaternion.identity);
        mafiaObj.gameObject.tag = deviceID;
        NetworkServer.SpawnWithClientAuthority(mafiaObj, gameObject);
    }
    
    public static PlayerController GetInstance()
    {
        return GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void AddForce(NetworkInstanceId obj, Vector2 startPos, Vector2 pos)
    {
        CmdAddForce(obj, startPos, pos);
    }

    [Command]
    public void CmdAddForce(NetworkInstanceId obj, Vector2 startPos, Vector2 pos)
    {
        NetworkServer.FindLocalObject(obj).GetComponent<Mafia>().AddForce(startPos, pos);
    }

    [Command]
    public void CmdOnSettleClicked(NetworkInstanceId shopID, NetworkInstanceId settledPlayerID)
    {
        ObjectManager.GetInstance().SettlePlayerInShop(settledPlayerID, shopID);
    }

    [Command]
    public void CmdOnAttackClicked(NetworkInstanceId attackPlayerID, NetworkInstanceId defencePlayerID)
    {
        ObjectManager.GetInstance().StartCombat(attackPlayerID, defencePlayerID);
    }

    [ClientRpc]
    public void RpcSettlePlayerInShop(NetworkInstanceId settledPlayerID, NetworkInstanceId shopID)
    {
        SettlePlayerInShop(settledPlayerID, shopID);
    }

    public void SettlePlayerInShop(NetworkInstanceId settledPlayerID, NetworkInstanceId shopID)
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
        settledPlayer.GetComponent<CombatUnit>().shopWhereIAm = shop;
    }

    [ClientRpc]
    public void RpcMafiaDead(NetworkInstanceId netID)
    {
        ClientScene.FindLocalObject(netID).GetComponent<CombatUnit>().Dead();
    }
}