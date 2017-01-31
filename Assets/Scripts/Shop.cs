using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class Shop : NetworkBehaviour, IPointerClickHandler
{
    public float deffenceBonus;
    public int xpBonus;
    public float heatMultiplier;
    public int heatHideValue;

    public GameObject UIAttack;

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
            FocusManager.SetFocusedBuilding(gameObject);
            UIAttack.GetComponent<AttackUIController>().ShowOnShop();
        }
    }

    public void OnSettleDead()
    {
        settledPlayer = null;
        settledPlayerNetId = NetworkInstanceId.Invalid;
    }

    public static void OnAttackClicked()
    {
        Shop shop = FocusManager.GetCurrentFocusedBuilding().GetComponent<Shop>();
        if (shop.settledPlayer != null)
            PlayerController.GetLocalInstance().CmdOnAttackClicked(FocusManager.GetCurrentFocusedPlayer().GetComponent<NetworkIdentity>().netId,
                shop.settledPlayer.GetComponent<NetworkIdentity>().netId);
        FocusManager.SetFocusedPlayer(null);
        FocusManager.SetFocusedBuilding(null);
    }

    public static void OnSettleClicked()
    {
        Shop shop = FocusManager.GetCurrentFocusedBuilding().GetComponent<Shop>();
        if (shop.settledPlayer == null)
        {
            PlayerController.GetLocalInstance().CmdOnSettleClicked(FocusManager.GetCurrentFocusedBuilding().GetComponent<NetworkIdentity>().netId,
                                FocusManager.GetCurrentFocusedPlayer().GetComponent<NetworkIdentity>().netId);
        }
        FocusManager.SetFocusedPlayer(null);
        FocusManager.SetFocusedBuilding(null);
    }

    void OnCollisionEnter(Collision col)
    {
        if (isServer && (heatHideValue < PoliceController.GetInstance().HeatPower) && (settledPlayer != null) && (col.gameObject.tag == "Vehicle"))
        {
            PoliceVehicle police = col.gameObject.GetComponent<PoliceVehicle>();
            if (police != null)
            {
                police.SpawnPolice(settledPlayer);
            }
        }
    }
}
