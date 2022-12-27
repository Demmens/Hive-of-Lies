using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientEventProvider : MonoBehaviour
{
    /// <summary>
    /// The sole client event provider
    /// </summary>
    public static ClientEventProvider singleton;

    public delegate void BasicEvent();
    public delegate void PlayerID(ulong playerID);
    public delegate void MissionResult(global::MissionResult result);

    /// <summary>
    /// Invoked when the mission ends
    /// </summary>
    public event MissionResult OnMissionEnd;

    /// <summary>
    /// Invoked when a client draws a card. Contains the draw result.
    /// </summary>
    public event PlayerDrew OnPlayerDrew;
    public delegate void PlayerDrew(DrawCardMsg msg);

    /// <summary>
    /// Invoked  when a player button is clicked on.
    /// </summary>
    public event PlayerID OnPlayerClicked;

    public event PlayerID OnTeamLeaderStartPicking;

    void Start()
    {
        singleton = this;
        DontDestroyOnLoad(this);

        //Register events
        NetworkClient.RegisterHandler((DrawCardMsg msg) => {OnPlayerDrew?.Invoke(msg); });
        NetworkClient.RegisterHandler((MissionEndMsg msg) => { OnMissionEnd?.Invoke(msg.result); });
    }
}