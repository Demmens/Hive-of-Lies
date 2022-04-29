using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerReady : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Handler registered");
        NetworkClient.RegisterHandler<TellAllPlayersReadyMsg>(AllPlayersReady);
    }

    void AllPlayersReady(TellAllPlayersReadyMsg msg)
    {
        Debug.Log("Heard Message");
        NetworkClient.Send(new PlayerReadyMsg() { playerID = SteamUser.GetSteamID() });
    }
}
