using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMission : MissionType
{
    #region Fields
    /// <summary>
    /// How many times players can roll for free
    /// </summary>
    [SerializeField] int freeRolls = 2;

    /// <summary>
    /// How much influence the first non-free reroll should cost
    /// </summary>
    [SerializeField] int firstRerollCost = 2;

    /// <summary>
    /// How many sides the dice you roll should have
    /// </summary>
    [SerializeField] int diceSize = 20;

    /// <summary>
    /// The minimum combined roll from all players needed to succeed the mission
    /// </summary>
    static int totalNeeded = 25;

    /// <summary>
    /// Prefab for the UI of this mission
    /// </summary>
    [SerializeField] GameObject UI;

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
    public delegate void PlayerRolled(Player ply, PlayerRollInfo roll);
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

    /// <summary>
    /// Delegate for <see cref="OnRerollCaculation"/>
    /// </summary>
    /// <param name="ply">The player rolling</param>
    /// <param name="cost">The base cost</param>
    /// <returns></returns>
    public delegate void RerollCalculation(Player ply, ref int cost);
    /// <summary>
    /// Invoked when the cost of rerolling is calculated. Allows subscribers to modify the result
    /// </summary>
    public event RerollCalculation OnRerollCaculation;

    #endregion

    public override void StartMission()
    {
        playersLocked = new List<Player>();
        rollTotal = 0;
        rollInfo = new Dictionary<Player, PlayerRollInfo>();
    }

    /// <summary>
    /// Determines how much the next reroll should cost
    /// </summary>
    /// <param name="numRerolls">How many times the player has rerolled so far</param>
    /// <returns></returns>
    int CalculateRerollCost(Player ply, int numRerolls)
    {
        //Make sure the correct number of rerolls are free
        if (numRerolls < freeRolls) return 0;

        //Calculate how many rerolls we have used that aren't free.
        numRerolls -= freeRolls;

        //Formula. Can edit this however we like for balance.
        int cost = firstRerollCost * (numRerolls+1);

        //Allow listeners to modify the cost
        OnRerollCaculation?.Invoke(ply, ref cost);

        return Mathf.Min(cost,0);
    }

    /// <summary>
    /// Called when a player uses a reroll
    /// </summary>
    /// <param name="ply">The player that rerolled</param>
    public void PlayerRerolled(Player ply)
    {
        //Assume they have 0 rerolls and their first roll is 0.
        PlayerRollInfo roll = new PlayerRollInfo()
        {
            rerollsUsed = 0,
            currentRoll = 0,
            locked = false
        };
        //If they have an entry in the dictionary, get that instead.
        rollInfo.TryGetValue(ply, out roll);

        //Make sure they haven't locked in their dice roll yet.
        if (roll.locked) return;

        int rerollCost = CalculateRerollCost(ply, roll.rerollsUsed);
        ply.Favour -= rerollCost;
        roll.rerollsUsed++;
        //Roll the dice
        roll.currentRoll = Random.Range(1, diceSize);

        rollInfo.Add(ply, roll);

        //Invoke the player rolled event
        OnPlayerRolled?.Invoke(ply,roll);
    }

    /// <summary>
    /// Called when a player locks in their dice roll.
    /// </summary>
    /// <param name="ply">The player that locked their roll</param>
    public void PlayerLockedIn(Player ply)
    {
        //Create rollinfo in case they never actually rolled their dice.
        PlayerRollInfo rollInfo = new PlayerRollInfo()
        {
            rerollsUsed = 0,
            currentRoll = 0,
        };
        this.rollInfo.TryGetValue(ply, out rollInfo);
        rollInfo.locked = true;
        rollTotal += rollInfo.currentRoll;
        playersLocked.Add(ply);

        //Invoke the player locked event
        OnPlayerLocked?.Invoke(ply);

        if (playersLocked.Count == Players.Count)
        {
            MissionResult result = (rollTotal >= totalNeeded) ? MissionResult.Success : MissionResult.Fail;

            //Invoke the all players locked event
            OnAllPlayersLocked?.Invoke(result);

            EndMission(result);
        }
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