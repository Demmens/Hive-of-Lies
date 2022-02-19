using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class Player
{
    #region Fields
    /// <summary>
    /// Private counterpart to <see cref="Favour"/>
    /// </summary>
    int favour = 0;

    /// <summary>
    /// Private counterpart to <see cref="Team"/>
    /// </summary>
    Team team = Team.Bee;

    /// <summary>
    /// Private counterpart to <see cref="DisplayName"/>
    /// </summary>
    string displayName;

    /// <summary>
    /// Private counterpart to <see cref="SteamID"/>
    /// </summary>
    CSteamID steamID;

    /// <summary>
    /// Private counterpart of <see cref="RoleChoices"/>
    /// </summary>
    List<RoleData> roleChoices;

    /// <summary>
    /// Private counterpart of <see cref="Conn"/>
    /// </summary>
    NetworkConnection conn;
    #endregion

    #region Properties
    /// <summary>
    /// The team that the player is on
    /// </summary>
    public Team Team
    {
        get
        {
            return team;
        }
        set
        {
            team = value;
        }
    }

    /// <summary>
    /// The favour the player has
    /// </summary>
    public int Favour
    {
        get
        {
            return favour;
        }
        set
        {
            favour = value;
        }
    }

    /// <summary>
    /// The display name of the player
    /// </summary>
    public string DisplayName
    {
        get
        {
            return displayName;
        }
        set
        {
            displayName = value;
        }
    }

    /// <summary>
    /// The steam ID of the player
    /// </summary>
    public CSteamID SteamID
    {
        get
        {
            return steamID;
        }
        set
        {
            steamID = SteamID;
        }
    }

    /// <summary>
    /// List of choices the player gets to choose from when picking their role.
    /// </summary>
    public List<RoleData> RoleChoices
    {
        get
        {
            return roleChoices;
        }
        set
        {
            roleChoices = value;
        }
    }

    /// <summary>
    /// The network connection of this player
    /// </summary>
    public NetworkConnection Conn
    {
        get
        {
            return conn;
        }
        set
        {
            conn = value;
        }
    }
    #endregion

    public Player(NetworkConnection conn, CSteamID id)
    {
        Conn = conn;
        SteamID = id;
        RoleChoices = new List<RoleData>();
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
