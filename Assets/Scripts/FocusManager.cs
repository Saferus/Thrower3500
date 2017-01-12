using UnityEngine;

public class FocusManager : MonoBehaviour {
    
    private static GameObject selectedPlayer;
    private static GameObject selectedBuilding;

    public static GameObject GetCurrentFocusedPlayer()
    {
        return selectedPlayer;
    }

    public static void SetFocusedPlayer(GameObject player)
    {
        selectedPlayer = player;
    }

    public static GameObject GetCurrentFocusedBuilding()
    {
        return selectedBuilding;
    }

    public static void SetFocusedBuilding(GameObject building)
    {
        selectedBuilding = building;
    }

    public static void OnAttackClicked()
    {
        selectedPlayer.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }
}
