using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class HostButton : MonoBehaviour
{
    private hiveNetworkManager networkManager
    {
        get
        {
            return NetworkManager.singleton as hiveNetworkManager;
        }
    }
    public void Click()
    {
        
    }
}
