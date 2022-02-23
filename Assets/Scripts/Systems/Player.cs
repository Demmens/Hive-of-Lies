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
    /// Private counterpart to <see cref="Exhaustion"/>
    /// </summary>
    int exhaustion;
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
            steamID = value;
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
    /// How many consecutive missions the player has been on
    /// </summary>
    public int Exhaustion
    {
        get
        {
            return exhaustion;
        }
        set
        {
            exhaustion = value;
        }
    }
    #endregion

    public Player(CSteamID id)
    {
        SteamID = id;
        DisplayName = SteamFriends.GetFriendPersonaName(id);
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
