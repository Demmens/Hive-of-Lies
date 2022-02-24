using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DiceMission : MissionType
{
    #region Fields

    /// <summary>
    /// How many sides the dice you roll should have
    /// </summary>
    [SerializeField] int diceSize = 20;

    /// <summary>
    /// How much exhaustion should affect the result of your roll
    /// </summary>
    [SerializeField] int exhaustionPenalty = 5;

    /// <summary>
    /// Reference to the cost calculation class
    /// </summary>
    [SerializeField] CostCalculation costCalc;

    /// <summary>
    /// The minimum combined roll from all players needed to succeed the mission
    /// </summary>
    public static int totalNeeded = 25;

    /// <summary>
    /// Contains information about the current state of each players' dice rolls.
    /// <para></para>
    /// Private counterpart to <see cref="RollInfo"/>
    /// </summary>
    Dictionary<Player, PlayerRollInfo> rollInfo;

    /// <summary>
    /// The total of all players' locked in rolls
    /// <para></para>
    /// Private counterpart to <see cref="RollTotal"/>
    /// </summary>
    int rollTotal;

    /// <summary>
    /// The players who have locked their rolls.
    /// <para></para>
    /// Private counterpart to <see cref="PlayersLocked"/>
    /// </summary>
    List<Player> playersLocked;

    #endregion

    #region Properties

    /// <summary>
    /// Contains information about the current state of each players' dice rolls.
    /// </summary>
    public Dictionary<Player, PlayerRollInfo> RollInfo
    {
        get
        {
            return rollInfo;
        }
        set
        {
            rollInfo = value;
        }
    }

    /// <summary>
    /// The total of all players' locked in rolls
    /// </summary>
    public int RollTotal
    {
        get
        {
            return rollTotal;
        }
        set
        {
            rollTotal = value;
        }
    }

    /// <summary>
    /// The players who have locked their rolls
    /// </summary>
    public List<Player> PlayersLocked
    {
        get
        {
            return playersLocked;
        }
        set
        {
            playersLocked = value;
        }
    }

    /// <summary>
    /// The minimum combined roll needed from all players to succeed the mission
    /// </summary>
    public static int TotalNeeded
    {
        get
        {
            return totalNeeded;
        }
        set
        {
            totalNeeded = value;
        }
    }
    
    #endregion

    #region Events

    /// <summary>
    /// Delegate for <see cref="OnPlayerRolled"/>
    /// </summary>
    /// <param name="ply">The player that rolled</param>
    /// <param name="roll">The roll information for that player</param>
    public delegate void PlayerRolled(Player ply, ref PlayerRollInfo roll);
    /// <summary>
    /// Invoked when a player rolls a dice
    /// </summary>
    public event PlayerRolled OnPlayerRolled;

    /// <summary>
    /// Delegate for <see cref="OnPlayerLocked"/>
    /// </summary>
    /// <param name="ply">The player who locked</param>
    public delegate void PlayerLocked(Player ply);
    /// <summary>
    /// Invoked when a player locks in their roll
    /// </summary>
    public event PlayerLocked OnPlayerLocked;

    /// <summary>
    /// Delegate for <see cref="OnAllPlayersLocked"/>
    /// </summary>
    /// <param name="result">The result the mission is going to have</param>
    public delegate void AllPlayersLocked(MissionResult result);
    /// <summary>
    /// Invoked once the final player has locked in their roll, but before the result is finalised
    /// </summary>
    public event AllPlayersLocked OnAllPlayersLocked;

    #endregion

    void Start()
    {
        NetworkServer.RegisterHandler<PlayerRolledMsg>(PlayerRerolled);
        NetworkServer.RegisterHandler<PlayerLockedRollMsg>(PlayerLockedIn);
    }

    public override void StartMission()
    {
        playersLocked = new List<Player>();
        rollTotal = 0;
        rollInfo = new Dictionary<Player, PlayerRollInfo>();
        NetworkServer.SendToAll(new DiceMissionStartedMsg() { });
    }

    /// <summary>
    /// Called when a player uses a reroll
    /// </summary>
    /// <param name="ply">The player that rerolled</param>
    void PlayerRerolled(NetworkConnection conn, PlayerRolledMsg msg)
    {
        if (!Active) return;
        GameInfo.Players.TryGetValue(conn, out Player ply);
        //Make sure they're actually on the mission
        if (!GameInfo.PlayersOnMission.Contains(ply)) return;
        bool exists = rollInfo.TryGetValue(ply, out PlayerRollInfo roll);
        //Make sure they haven't locked in their dice roll yet.
        if (roll.locked) return;
        int rerollCost = costCalc.CalculateRerollCost(ply.SteamID, roll.rerollsUsed);
        //Make sure they can afford the reroll
        if (rerollCost > ply.Favour) return;


        ply.Favour -= rerollCost;
        roll.rerollsUsed++;
        //Roll the dice and apply exhaustion penalty
        roll.currentRoll = Mathf.Max(1,Random.Range(1, diceSize) - ply.Exhaustion * exhaustionPenalty);

        //Invoke the player rolled event
        OnPlayerRolled?.Invoke(ply,ref roll);

        if (exists) rollInfo[ply] = roll;
        else rollInfo.Add(ply, roll);

        msg.rollResult = roll.currentRoll;
        conn.Send(msg);
    }

    /// <summary>
    /// Called when a player locks in their dice roll.
    /// </summary>
    /// <param name="ply">The player that locked their roll</param>
    void PlayerLockedIn(NetworkConnection conn, PlayerLockedRollMsg msg)
    {
        if (!Active) return;
        GameInfo.Players.TryGetValue(conn, out Player ply);

        //Make sure they're actually on the mission
        if (!GameInfo.PlayersOnMission.Contains(ply)) return;

        rollInfo.TryGetValue(ply, out var plyRollInfo);

        //Make sure they're not already locked.
        if (plyRollInfo.locked) return;

        plyRollInfo.locked = true;
        rollTotal += plyRollInfo.currentRoll;
        playersLocked.Add(ply);

        //Invoke the player locked event
        OnPlayerLocked?.Invoke(ply);

        if (playersLocked.Count == Players.Count)
        {
            AllPlayersSubmitted();
        }
    }

    void AllPlayersSubmitted()
    {
        MissionResult result = (rollTotal >= totalNeeded) ? MissionResult.Success : MissionResult.Fail;

        //Invoke the all players locked event
        OnAllPlayersLocked?.Invoke(result);

        List<int> finalRolls = new List<int>();
        foreach (KeyValuePair<Player,PlayerRollInfo> roll in rollInfo)
        {
            finalRolls.Add(roll.Value.currentRoll);
        }

        SendMissionPlayersRollsMsg msg = new SendMissionPlayersRollsMsg()
        {
            finalRolls = finalRolls
        };

        foreach (KeyValuePair<NetworkConnection,Player> pair in GameInfo.Players)
        {

        }

        EndMission(result);
    }
}

/// <summary>
/// Information about a players dice rolls
/// </summary>
public struct PlayerRollInfo
{
    /// <summary>
    /// The value of the players current roll
    /// </summary>
    public int currentRoll;
    /// <summary>
    /// How many times the player has rerolled their dice
    /// </summary>
    public int rerollsUsed;
    /// <summary>
    /// Whether the player has locked in their roll yet
    /// </summary>
    public bool locked;
}

public struct DiceMissionStartedMsg : NetworkMessage
{
    
}

public struct PlayerRolledMsg : NetworkMessage
{
    public int rollResult;
}

public struct PlayerLockedRollMsg : NetworkMessage
{
    public bool lastPlayer;
}

public struct SendMissionPlayersRollsMsg : NetworkMessage
{
    public List<int> finalRolls;
}