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
    /// Invoked when the team leader changes
    /// </summary>
    public event TeamLeaderChanged OnTeamLeaderChanged;
    public delegate void TeamLeaderChanged(TeamLeaderChangedMsg msg);

    /// <summary>
    /// Invoked when a client draws a card. Contains the draw result.
    /// </summary>
    public event PlayerDrew OnPlayerDrew;
    public delegate void PlayerDrew(DrawCardMsg msg);

    /// <summary>
    /// Invoked when a client rolls a dice. Contains the roll result.
    /// </summary>
    public event PlayerRolled OnPlayerRolled;
    public delegate void PlayerRolled(PlayerRolledMsg msg);

    /// <summary>
    /// Invoked  when a player button is clicked on.
    /// </summary>
    public event PlayerID OnPlayerClicked;

    public event PlayerID OnTeamLeaderStartPicking;

    /// <summary>
    /// Invoked when the team leader changes their partner choice
    /// </summary>
    public event TeamLeaderChangedPartner OnTeamLeaderChangePartner;
    public delegate void TeamLeaderChangedPartner(TeamLeaderChangePartnersMsg msg);

    void Start()
    {
        singleton = this;
        DontDestroyOnLoad(this);

        //Register events
        NetworkClient.RegisterHandler((TeamLeaderChangedMsg msg) => { OnTeamLeaderChanged?.Invoke(msg); });
        NetworkClient.RegisterHandler((DrawCardMsg msg) => {OnPlayerDrew?.Invoke(msg); });
        NetworkClient.RegisterHandler((PlayerRolledMsg msg) => { OnPlayerRolled?.Invoke(msg); });
        NetworkClient.RegisterHandler((MissionEndMsg msg) => { OnMissionEnd?.Invoke(msg.result); });
        NetworkClient.RegisterHandler((TeamLeaderStartPickingMsg msg) => { OnTeamLeaderStartPicking?.Invoke(msg.teamLeaderID); });
        NetworkClient.RegisterHandler((TeamLeaderChangePartnersMsg msg) => { OnTeamLeaderChangePartner?.Invoke(msg); });
    }

    public void ClickPlayer(ulong playerID)
    {
        OnPlayerClicked?.Invoke(playerID);
    }
}