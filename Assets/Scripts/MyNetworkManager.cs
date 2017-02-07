﻿using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (Network.isServer)
        {
            NetworkServer.Spawn(GameObject.Find("ObjectManager"));
            NetworkServer.Spawn(GameObject.Find("Buildings"));
            NetworkServer.Spawn(GameObject.Find("Road"));
        }
    }
}
