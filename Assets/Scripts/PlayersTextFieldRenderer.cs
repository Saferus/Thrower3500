using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayersTextFieldRenderer : NetworkBehaviour {

    public GameObject text;

    // Use this for initialization
    void Start ()
    {
        if (!isServer)
        {
            Destroy(this);
            return;
        }
        text = gameObject;
        Instantiate(text, Vector3.zero, Quaternion.identity);
        //text.transform.Find("PlayersInfo").GetComponent<Text>().text = "init";
    }
	
	// Update is called once per frame
	void Update ()
    {
        string info = PlayerManager.GetInstance().GetPlayersInfo();
        text.transform.Find("PlayersInfo").GetComponentInChildren<Text>().text = info;
    }
}
