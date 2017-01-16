using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject mafiaPrefab;

    public override void OnStartLocalPlayer()
    {
        CmdPlayerConnected(SystemInfo.deviceUniqueIdentifier);
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
}