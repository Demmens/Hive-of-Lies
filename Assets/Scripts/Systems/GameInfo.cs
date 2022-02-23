using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

/// <summary>
/// Contains basic information about the current game
/// </summary>
public class GameInfo : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the game end script
    /// </summary>
    [SerializeField] GameEnd end;

    /// <summary>
    /// Private counterpart to <see cref="StandingTeamLeader"/>
    /// </summary>
    private Player standingTeamLeader;

    /// <summary>
    /// Private counterpart to <see cref="Roles"/>
    /// </summary>
    private List<Role> roles = new List<Role>();

    #endregion

    #region Properties

    /// <summary>
    /// The player that is currently being voted to become the team leader.
    /// </summary>
    public Player StandingTeamLeader
    {
        get
        {
            return standingTeamLeader;
        }
        set
        {
            standingTeamLeader = StandingTeamLeader;
        }
    }

    /// <summary>
    /// Progress on researching wasps. 3 research wins the game.
    /// </summary>
    public int WaspResearch
    {
        get
        {
            return waspResearch;
        }
        set
        {
            waspResearch = Mathf.Clamp(value,0,end.ResearchNeededForWin);
            if (waspResearch == end.ResearchNeededForWin)
            {
                end.EndGame(Team.Bee);
            }
        }
    }

    /// <summary>
    /// Progress on researching wasps. 3 research wins the game.
    /// </summary>
    public int HoneyStolen
    {
        get
        {
            return honeyStolen;
        }
        set
        {
            honeyStolen = Mathf.Clamp(value,0,end.HoneyNeededForWin);
            if (honeyStolen == end.HoneyNeededForWin)
            {
                end.EndGame(Team.Wasp);
            }
        }
    }

    /// <summary>
    /// All roles and their instantiated abilities
    /// </summary>
    public List<Role> Roles
    {
        get
        {
            return roles;
        }
        set
        {
            roles = value;
        }
    }

    /// <summary>
    /// Map each CSteamID to the respective player. Makes it a lot easier to find players
    /// </summary>
    public static Dictionary<NetworkConnection, Player> Players = new Dictionary<NetworkConnection, Player>();

    #endregion

    #region Static Fields and Properties

    /// <summary>
    /// Current progress for researching wasps. 3 research wins the game for the Bees.
    /// </summary>
    public static int waspResearch { get; private set; }

    /// <summary>
    /// How much honey has been stolen. 3 stolen honey wins the game for the wasps.
    /// </summary>
    public static int honeyStolen { get; private set; }

    /// <summary>
    /// Current round number
    /// </summary>
    public static int RoundNum;

    /// <summary>
    /// Current phase of the game
    /// </summary>
    public static EGamePhase GamePhase;

    /// <summary>
    /// Player count for this game
    /// </summary>
    public static int PlayerCount;

    /// <summary>
    /// The player ID of the team leader
    /// </summary>
    public static CSteamID TeamLeaderID { get; private set; }

    /// <summary>
    /// The current team leader
    /// </summary>
    public static Player TeamLeader;

    /// <summary>
    /// A list of all players that are on the mission
    /// </summary>
    public static List<Player> PlayersOnMission = new List<Player>();

    /// <summary>
    /// The list of missions we are using for this game
    /// </summary>
    public static MissionList MissionList;

    /// <summary>
    /// The current mission
    /// </summary>
    public static Mission CurrentMission;

    #endregion

    private void Start()
    {
        PlayerCount = SteamMatchmaking.GetNumLobbyMembers(SteamLobby.LobbyID);
        Debug.Log($"Started game with {PlayerCount} players");
    }
}

/// <summary>
/// All the information of a role. Contains RoleData and instantiated RoleAbility.
/// </summary>
public class Role
{
    /// <summary>
    /// Data of the role
    /// </summary>
    public RoleData Data;
    /// <summary>
    /// Ability of the role
    /// </summary>
    public RoleAbility Ability;
}