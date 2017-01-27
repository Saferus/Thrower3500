using UnityEngine;

public class PoliceController : MonoBehaviour
{
    public GameObject PolicePrefab;
    public float heatDamageMultiplier;
    public float heatExtinctionMultiplier;

    public int HeatPower { get; set; }
    private static PoliceController instance;

    void Start ()
    {
        instance = GameObject.Find("PoliceManager").GetComponent<PoliceController>();
    }

    void Update ()
	{
	    HeatPower -= (int) (Time.deltaTime * heatExtinctionMultiplier);
    }

    public static PoliceController GetInstance()
    {
        return instance;
    }

    public void ShopOccupied(GameObject shop)
    {
        HeatPower += (int)(Time.deltaTime * shop.GetComponent<Shop>().heatMultiplier);
    }

    public void DamageDealed(int count)
    {
        HeatPower += (int)(count * heatDamageMultiplier);
    }
}
