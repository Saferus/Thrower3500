using UnityEngine;
using UnityEngine.Networking;

public class PoliceVehicle : MonoBehaviour
{
    public int policeSpawnCD = 10;
    public float spawnCD = 0;
    public GameObject policePrefab;

    void Update()
    {
        if (spawnCD > 0)
            spawnCD -= Time.deltaTime;
    }

    public void SpawnPolice(GameObject mafia)
    {
        if (spawnCD <= 0)
        {
            spawnCD = policeSpawnCD;
            GameObject policeUnit = Instantiate(policePrefab, mafia.transform.position, Quaternion.identity);
            NetworkServer.Spawn(policeUnit);
            ObjectManager.GetInstance().StartCombat(policeUnit.GetComponent<NetworkIdentity>().netId, mafia.GetComponent<NetworkIdentity>().netId);
        }
    }
}
