using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayersTextFieldRenderer : NetworkBehaviour {

    public GameObject text;
    
    void Start ()
    {
        if (!isServer)
        {
            Destroy(this);
            return;
        }
        text = Instantiate(text, Vector3.zero, Quaternion.identity);
        text.transform.Find("PlayersInfo").GetComponent<Text>().text = "init";
    }
	
	void Update ()
    {
        if (PoliceController.GetInstance() != null)
        {
            string info = PlayerManager.GetInstance().GetPlayersInfo();
            info += "HeatPower = " + PoliceController.GetInstance().HeatPower;
            text.transform.Find("PlayersInfo").GetComponent<Text>().text = info;
        }
    }
}
