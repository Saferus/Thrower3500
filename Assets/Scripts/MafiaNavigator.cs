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
        NetworkIdentity netID = other.gameObject.GetComponent<NetworkIdentity>();
        if (isServer && (other.gameObject == shop || (netID && netID.netId == mafiaID)))
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
        this.shop = shop;

        Collider shopCollider = shop.GetComponent<Collider>();
        if (shopCollider.bounds.Intersects(gameObject.GetComponent<Collider>().bounds))
        {
            OnTriggerEnter(shopCollider);
            return;
        }

        gameObject.GetComponent<NavMeshAgent>().SetDestination(shop.transform.FindChild("roadPoint").transform.position);
        gameObject.GetComponent<NavMeshAgent>().Resume();
    }

    public void MoveToMafia(NetworkInstanceId mafiaID, Vector3 pos)
    {
        this.mafiaID = mafiaID;

        GameObject enemy = NetworkServer.FindLocalObject(mafiaID);
        if (Vector3.Distance(enemy.transform.position, gameObject.transform.position) <= 1.5f)
        {
            OnTriggerEnter(enemy.GetComponent<Collider>());
            return;
        }

        gameObject.GetComponent<NavMeshAgent>().SetDestination(pos);
        gameObject.GetComponent<NavMeshAgent>().Resume();
    }

    public void MoveToMafiaInShop(NetworkInstanceId mafiaID, GameObject shop)
    {
        this.mafiaID = mafiaID;
        this.shop = shop;

        Collider shopCollider = shop.GetComponent<Collider>();
        if (shopCollider.bounds.Intersects(gameObject.GetComponent<Collider>().bounds))
        {
            OnTriggerEnter(shopCollider);
            return;
        }

        gameObject.GetComponent<NavMeshAgent>().SetDestination(shop.transform.FindChild("roadPoint").transform.position);
        gameObject.GetComponent<NavMeshAgent>().Resume();
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
