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

    public int test = 5;

    public delegate void BasicEvent();

    public event BasicEvent OnMissionEnd;

    /// <summary>
    /// Fired when the team leader changes
    /// </summary>
    public event TeamLeaderChanged OnTeamLeaderChanged;
    public delegate void TeamLeaderChanged(TeamLeaderChangedMsg msg);

    /// <summary>
    /// Fired when this client rolls a dice. Contains the roll result.
    /// </summary>
    public event PlayerRolled OnPlayerRolled;
    public delegate void PlayerRolled(PlayerRolledMsg msg);

    void Start()
    {
        singleton = this;
        DontDestroyOnLoad(this);

        //Register events
        NetworkClient.RegisterHandler((TeamLeaderChangedMsg msg) => { OnTeamLeaderChanged?.Invoke(msg); });
        NetworkClient.RegisterHandler((PlayerRolledMsg msg) => { OnPlayerRolled?.Invoke(msg); });
        NetworkClient.RegisterHandler((MissionEndMsg msg) => { OnMissionEnd?.Invoke(); });
    }
}
