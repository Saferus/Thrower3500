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

    private GameObject shop;
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
        if (isServer && (other.gameObject == shop || other.gameObject.GetComponent<NetworkIdentity>().netId == mafiaID))
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

    
    public void MoveToShop(GameObject shop)
    {
        Collider collider = shop.GetComponent<Collider>();
        if (collider.bounds.Intersects(gameObject.GetComponent<Collider>().bounds))/// Contains(gameObject.transform.position))
        {
            OnTriggerEnter(collider);
            return;
        }
        gameObject.GetComponent<NavMeshAgent>().SetDestination(shop.transform.FindChild("roadPoint").transform.position);
        this.shop = shop;
    }

    public void MoveToMafia(NetworkInstanceId mafiaID, Vector3 pos)
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(pos);
        this.mafiaID = mafiaID;
    }

    public void MoveToMafiaInShop(NetworkInstanceId mafiaID, GameObject shop)
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(shop.transform.FindChild("roadPoint").transform.position);
        this.mafiaID = mafiaID;
        this.shop = shop;
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
        if (shop.GetComponent<Shop>().settledPlayer == null)
        {
            Destroy(gameObject.GetComponent<NavMeshAgent>());
            ObjectManager.GetInstance().SettlePlayerInShop(gameObject.GetComponent<NetworkIdentity>().netId, shop.GetComponent<NetworkIdentity>().netId);
            gameObject.transform.Translate(shop.transform.position);
            Destroy(this);
        }
    }

    private void Attack()
    {
        ObjectManager.GetInstance().StartCombat(gameObject.GetComponent<NetworkIdentity>().netId, mafiaID);
    }

}
