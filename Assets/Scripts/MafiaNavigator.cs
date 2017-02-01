using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class MafiaNavigator : NetworkBehaviour
{

    enum Action
    {
        SETTLE,
        ATTACK,
        NONE
    }

    private Action postAction = Action.NONE;

    private NetworkInstanceId targetID;

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

            switch (postAction)
            {
                case Action.SETTLE:
                {
                    Settle();
                    break;
                }
                case Action.ATTACK:
                {
                    Attack();
                    break;
                }
            }

            postAction = Action.NONE;
        }
    }

    
    public void MoveToTarget(NetworkInstanceId targetID)
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(NetworkServer.FindLocalObject(targetID).transform.FindChild("roadPoint").transform.position);
        this.targetID = targetID;
    }

    public void WaitSettle()
    {
        postAction = Action.SETTLE;
    }

    public void WaitAttack()
    {
        postAction = Action.ATTACK;
    }

    private void Settle()
    {
        ObjectManager.GetInstance().SettlePlayerInShop(gameObject.GetComponent<NetworkIdentity>().netId, targetID);
    }

    private void Attack()
    {
        ObjectManager.GetInstance().StartCombat(gameObject.GetComponent<NetworkIdentity>().netId, targetID);
    }

}
