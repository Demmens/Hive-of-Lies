using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public static class ClientGameInfo
{
    /// <summary>
    /// The current phase of the game
    /// </summary>
    public static GamePhase GamePhase;

    /// <summary>
    /// The steam ID of the team leader
    /// </summary>
    public static CSteamID TeamLeaderID;

    /// <summary>
    /// The ID of the local player
    /// </summary>
    public static ulong PlayerID = SteamUser.GetSteamID().m_SteamID;

    /// <summary>
    /// The number of players the team leader has currently selected for the mission
    /// </summary>
    public static List<ulong> CurrentlySelected = new List<ulong>();

    /// <summary>
    /// The total number of players that can be chosen to go on the mission
    /// </summary>
    public static int MaxPartners;
}
