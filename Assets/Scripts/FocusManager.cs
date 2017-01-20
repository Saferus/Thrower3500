using UnityEngine;

public class FocusManager : MonoBehaviour {

    private static GameObject focusedPlayer;
    private static GameObject focusedEnemy;
    private static GameObject focusedBuilding;

    public static GameObject GetCurrentFocusedPlayer()
    {
        return focusedPlayer;
    }

    public static GameObject GetCurrentFocusedEnemy()
    {
        return focusedEnemy;
    }

    public static void SetFocusedPlayer(GameObject player)
    {
        if (focusedPlayer != null)
            focusedPlayer.GetComponent<Renderer>().material.color = Color.green;
        focusedPlayer = player;
        if (focusedPlayer != null)
            focusedPlayer.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public static void SetFocusedEnemy(GameObject player)
    {
        focusedEnemy = player;
    }

    public static GameObject GetCurrentFocusedBuilding()
    {
        return focusedBuilding;
    }

    public static void SetFocusedBuilding(GameObject building)
    {
        focusedBuilding = building;
    }
}
