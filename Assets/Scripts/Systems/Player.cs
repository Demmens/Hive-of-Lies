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
    /// The ulong ID of the player
    /// </summary>
    public ulong ID;

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

    /// <summary>
    /// The target of the player (if they're a traitor)
    /// </summary>
    public Player target;

    public Player(CSteamID id, NetworkConnection conn)
    {
        SteamID = id;
        ID = id.m_SteamID;
        Connection = conn;
        DisplayName = SteamFriends.GetFriendPersonaName(id);
        RoleChoices = new List<RoleData>();
    }
    public Player(ulong id, NetworkConnection conn)
    {
        SteamID = new CSteamID(id);
        ID = id;
        Connection = conn;
        DisplayName = SteamFriends.GetFriendPersonaName(SteamID);
        RoleChoices = new List<RoleData>();
    }

    public Player()
    {
        if (NetworkClient.active)
        {
            SteamID = SteamUser.GetSteamID();
            ID = SteamID.m_SteamID;
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
