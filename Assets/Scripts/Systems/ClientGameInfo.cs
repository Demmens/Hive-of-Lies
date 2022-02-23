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
    /// The number of players the team leader has currently selected for the mission
    /// </summary>
    public static List<CSteamID> CurrentlySelected = new List<CSteamID>();

    /// <summary>
    /// The total number of players that can be chosen to go on the mission
    /// </summary>
    public static int MaxPartners;
}
