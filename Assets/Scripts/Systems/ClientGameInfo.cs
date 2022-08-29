using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class ClientGameInfo : MonoBehaviour
{
    public static ClientGameInfo singleton;

    /// <summary>
    /// The current phase of the game
    /// </summary>
    public GamePhase GamePhase;

    /// <summary>
    /// The steam ID of the team leader
    /// </summary>
    public CSteamID TeamLeaderID;

    /// <summary>
    /// The ID of the local player
    /// </summary>
    public ulong PlayerID;

    /// <summary>
    /// The number of players the team leader has currently selected for the mission
    /// </summary>
    public List<ulong> CurrentlySelected = new List<ulong>();

    /// <summary>
    /// The total number of players that can be chosen to go on the mission
    /// </summary>
    public int MaxPartners;

    /// <summary>
    /// Whether the player should be included in the game.
    /// </summary>
    public bool Alive = true;

    private void Start()
    {
        singleton = this;
        DontDestroyOnLoad(this);
        PlayerID = SteamUser.GetSteamID().m_SteamID;
    }
}
