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

    private NetworkInstanceId shopID;
    private NetworkInstanceId mafiaID;

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
        if (isServer && (other.gameObject.GetComponent<NetworkIdentity>().netId == shopID 
            || other.gameObject.GetComponent<NetworkIdentity>().netId == mafiaID))
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

    
    public void MoveToShop(NetworkInstanceId shopID, Vector3 pos)
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(pos);
        this.shopID = shopID;
    }

    public void MoveToMafia(NetworkInstanceId mafiaID, Vector3 pos)
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(pos);
        this.mafiaID = mafiaID;
    }

    public void MoveToMafiaInShop(NetworkInstanceId mafiaID, NetworkInstanceId shopID, Vector3 pos)
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(pos);
        this.mafiaID = mafiaID;
        this.shopID = shopID;
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
        ObjectManager.GetInstance().SettlePlayerInShop(gameObject.GetComponent<NetworkIdentity>().netId, shopID);
        gameObject.transform.Translate(NetworkServer.FindLocalObject(shopID).transform.position);
        Destroy(this);
    }

    private void Attack()
    {
        ObjectManager.GetInstance().StartCombat(gameObject.GetComponent<NetworkIdentity>().netId, mafiaID);
    }

}
