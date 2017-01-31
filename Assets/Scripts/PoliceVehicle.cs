using UnityEngine;
using UnityEngine.Networking;

public class PoliceVehicle : MonoBehaviour
{

    public GameObject policePrefab;

    public void SpawnPolice(GameObject mafia)
    {
        GameObject policeUnit = Instantiate(policePrefab, mafia.transform.position, Quaternion.identity);
        NetworkServer.Spawn(policeUnit);
        ObjectManager.GetInstance().StartCombat(policeUnit.GetComponent<NetworkIdentity>().netId, mafia.GetComponent<NetworkIdentity>().netId);
    }
}
