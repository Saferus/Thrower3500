using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject mafiaPrefab;
    private static PlayerController localController;

    public static PlayerController GetLocalInstance()
    {
        return localController;
    }

    public override void OnStartLocalPlayer()
    {
        CmdPlayerConnected(SystemInfo.deviceUniqueIdentifier);
        localController = this;
    }

    [Command]
    public void CmdPlayerConnected(string deviceID)
    {
        PlayerManager.GetInstance().PlayerConnected(deviceID);
    }

    public void SpawnMafia(string deviceID)
    {
        GameObject mafiaObj = (GameObject) Instantiate(mafiaPrefab, Vector3.zero, Quaternion.identity);
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

    [ClientRpc]
    public void RpcMafiaDead(NetworkInstanceId netID)
    {
        ClientScene.FindLocalObject(netID).GetComponent<Mafia>().Dead();
    }
}