using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// Private counterpart to <see cref="General"/>
    /// </summary>
    private Player teamLeader;

    /// <summary>
    /// Private counterpart to <see cref="StandingTeamLeader"/>
    /// </summary>
    private Player standingTeamLeader;

    /// <summary>
    /// Private counterpart to <see cref="CurrentMission"/>
    /// </summary>
    private Mission currentMission;

    /// <summary>
    /// Private counterpart to <see cref="MissionList"/>
    /// </summary>
    private MissionList missionList;

    /// <summary>
    /// Private counterpart to <see cref="Roles"/>
    /// </summary>
    private List<Role> roles; 

    #endregion

    #region Properties
    /// <summary>
    /// The player that is the team leader of the current mission.
    /// </summary>
    public Player TeamLeader
    {
        get
        {
            return teamLeader;
        }
        set
        {
            teamLeader = value;
            TeamLeaderID = value == null ? -1 : value.ID;
        }
    }

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
    /// The current mission
    /// </summary>
    public Mission CurrentMission
    {
        get
        {
            return currentMission;
        }
        set
        {
            currentMission = CurrentMission;
        }
    }

    /// <summary>
    /// The list of missions we are using for this game
    /// </summary>
    public MissionList MissionList
    {
        get
        {
            return missionList;
        }
        set
        {
            missionList = MissionList;
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
    public static int TeamLeaderID { get; private set; }

    #endregion
}

/// <summary>
/// All the information of a role. Contains RoleData and instantiated RoleAbility.
/// </summary>
public struct Role
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