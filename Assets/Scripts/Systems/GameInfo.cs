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
    public static GameInfo singleton;

    /// <summary>
    /// The player that is currently being voted to become the team leader.
    /// </summary>
    public Player StandingTeamLeader;

    /// <summary>
    /// Current progress for researching wasps. 3 research wins the game for the Bees.
    /// </summary>
    private int waspResearch = 0;

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
            waspResearch = Mathf.Clamp(value, 0, GameEnd.singleton.ResearchNeededForWin);
            if (waspResearch == GameEnd.singleton.ResearchNeededForWin)
            {
                GameEnd.singleton.EndGame(Team.Bee);
            }
        }
    }

    /// <summary>
    /// How much honey has been stolen. 3 stolen honey wins the game for the wasps.
    /// </summary>
    private int honeyStolen = 0;

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
            honeyStolen = Mathf.Clamp(value, 0, GameEnd.singleton.HoneyNeededForWin);
            if (honeyStolen == GameEnd.singleton.HoneyNeededForWin)
            {
                GameEnd.singleton.EndGame(Team.Wasp);
            }
        }
    }

    /// <summary>
    /// All roles and their instantiated abilities
    /// </summary>
    public List<Role> Roles = new List<Role>();

    /// <summary>
    /// Map each CSteamID to the respective player. Makes it a lot easier to find players
    /// </summary>
    public Dictionary<NetworkConnection, Player> Players = new Dictionary<NetworkConnection, Player>();

    /// <summary>
    /// Current round number
    /// </summary>
    public int RoundNum;

    /// <summary>
    /// Current phase of the game
    /// </summary>
    public EGamePhase GamePhase;

    /// <summary>
    /// Player count for this game
    /// </summary>
    public int PlayerCount;

    /// <summary>
    /// The player ID of the team leader
    /// </summary>
    public CSteamID TeamLeaderID { get; private set; }

    /// <summary>
    /// The current team leader
    /// </summary>
    public Player TeamLeader;

    /// <summary>
    /// A list of all players that are on the mission
    /// </summary>
    public List<Player> PlayersOnMission = new List<Player>();

    /// <summary>
    /// The list of missions we are using for this game
    /// </summary>
    public MissionList MissionList;

    /// <summary>
    /// The current mission
    /// </summary>
    public Mission CurrentMission;

    /// <summary>
    /// All completed missions
    /// </summary>
    public Dictionary<Mission, MissionResult> CompletedMissions = new Dictionary<Mission, MissionResult>();

    private void Start()
    {
        singleton = this;
        DontDestroyOnLoad(this);
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