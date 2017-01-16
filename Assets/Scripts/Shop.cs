using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Shop : NetworkBehaviour, IPointerClickHandler {

    public GameObject UIAttack;
    public Vector3 uiPos;
    private GameObject attackUI;

    [SyncVar]
    public NetworkInstanceId settledPlayerNetId = NetworkInstanceId.Invalid;
    public GameObject settledPlayer;

    public void SynchronizeData()
    {
        if (settledPlayerNetId != NetworkInstanceId.Invalid)
            PlayerController.GetLocalInstance()
                .SettlePlayerInShop(settledPlayerNetId, GetComponent<NetworkIdentity>().netId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (FocusManager.GetCurrentFocusedPlayer() != null)
        {
            Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);
            attackUI = (GameObject)Instantiate(UIAttack, uiPos, rot);
            GameObject.Find("Attack").GetComponent<Button>().onClick.AddListener(() => FocusManager.GetCurrentFocusedPlayer().GetComponent<Mafia>().OnAttackClicked());
            GameObject.Find("Settle").GetComponent<Button>().onClick.AddListener(() => FocusManager.GetCurrentFocusedPlayer().GetComponent<Mafia>().OnSettleClicked());
            FocusManager.SetFocusedBuilding(gameObject);
        }
    }

    public void HideUI()
    {
        Destroy(attackUI);
    }

    public void OnSettleDead()
    {
        settledPlayer = null;
        settledPlayerNetId = NetworkInstanceId.Invalid;
    }
}
