using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class GameLoaded : MonoBehaviour
{
    void Start()
    {
        NetworkClient.Send(new PlayerReadyMsg()
        {
            playerID = SteamUser.GetSteamID()
        });
    }
}
