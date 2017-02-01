using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MafiaNavigator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "NormalShop")
        {
            gameObject.GetComponent<NavMeshAgent>().Stop();
        }
    }
}
