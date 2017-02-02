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
    void Start ()
    {
        if (!isServer)
        {
            Destroy(gameObject.GetComponent<NavMeshAgent>());
            Destroy(this);
        }
    }
	
	// Update is called once per frame
	void Update () {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isServer && other.gameObject.GetComponent<NetworkIdentity>().netId == targetID)
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

    
    public void MoveToTarget(NetworkInstanceId targetID, Vector3 pos)
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(pos);
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
        Destroy(gameObject.GetComponent<NavMeshAgent>());
        ObjectManager.GetInstance().SettlePlayerInShop(gameObject.GetComponent<NetworkIdentity>().netId, targetID);
        gameObject.transform.Translate(NetworkServer.FindLocalObject(targetID).transform.position);
        Destroy(this);
    }

    private void Attack()
    {
        ObjectManager.GetInstance().StartCombat(gameObject.GetComponent<NetworkIdentity>().netId, targetID);
    }

}
