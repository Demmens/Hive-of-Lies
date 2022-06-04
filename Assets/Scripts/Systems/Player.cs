using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class Player
{

    /// <summary>
    /// The team that the player is on
    /// </summary>
    public Team Team = Team.Bee;

    /// <summary>
    /// The favour the player has
    /// </summary>
    public int Favour;

    /// <summary>
    /// The display name of the player
    /// </summary>
    public string DisplayName;

    /// <summary>
    /// The steam ID of the player
    /// </summary>
    public CSteamID SteamID;

    /// <summary>
    /// List of choices the player gets to choose from when picking their role.
    /// </summary>
    public List<RoleData> RoleChoices;

    /// <summary>
    /// How many consecutive missions the player has been on
    /// </summary>
    public int Exhaustion;

    /// <summary>
    /// The clients connection
    /// </summary>
    public NetworkConnection Connection;

    public Player(CSteamID id, NetworkConnection conn)
    {
        SteamID = id;
        Connection = conn;
        DisplayName = SteamFriends.GetFriendPersonaName(id);
        RoleChoices = new List<RoleData>();
    }

    public Player()
    {
        if (NetworkClient.active)
        {
            SteamID = SteamUser.GetSteamID();
            DisplayName = SteamFriends.GetPersonaName();
        }
        
    }
}

/// <summary>
/// Innocent or Traitor. Currently no plans for more but may rename later.
/// </summary>
public enum Team
{
    None,
    Bee,
    Wasp,
}
