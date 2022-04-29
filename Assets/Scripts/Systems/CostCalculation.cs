using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class CostCalculation : NetworkBehaviour
{
    public static CostCalculation singleton;
    /// <summary>
    /// Number of rolls the player gets to make for free. Should always be at least 1.
    /// </summary>
    [SyncVar] public int FreeRolls = 2;
    /// <summary>
    /// Cost of the first paid reroll
    /// </summary>
    [SyncVar] public int FirstRerollCost = 4;
    /// <summary>
    /// How much to increase all roll costs by
    /// </summary>
    [SyncVar] public int GlobalRollCostMod = 0;

    /// <summary>
    /// Number times the player may draw a card for free. Should always be at least 1.
    /// </summary>
    [SyncVar] public int FreeDraws = 2;

    /// <summary>
    /// Cost of the first paiud draw
    /// </summary>
    [SyncVar] public int FirstDrawCost = 4;

    /// <summary>
    /// How much to increase all draw costs by
    /// </summary>
    [SyncVar] public int GlobalDrawCostMod = 0;

    /// <summary>
    /// Sends vote cost information out for modification
    /// </summary>
    public event VoteCalculation OnVoteCalculation;
    public delegate void VoteCalculation(CSteamID ply, ref int cost);

    /// <summary>
    /// Sends roll cost information out for modification
    /// </summary>
    public event RerollCalculation OnRerollCalculation;
    public delegate void RerollCalculation(CSteamID ply, ref int cost);

    /// <summary>
    /// Sends draw cost information out for modification
    /// </summary>
    public event DrawCalculation OnDrawCalculation;
    public delegate void DrawCalculation(ulong ply, ref int cost);

    void Start()
    {
        singleton = this;
    }

    /// <summary>
    /// Calculate the favour cost of the vote
    /// </summary>
    /// <param name="ply">The player who's voting</param>
    /// <param name="numVotes">The number of votes</param>
    public int CalculateVoteCost(CSteamID ply, int numVotes)
    {
        //Votes cost the same up and down
        numVotes = Mathf.Abs(numVotes);
        // Cost is 2n-1 We can change this formula at any time for balance
        // Gives us the costs: {0,0,2,4,6,8}
        // Cumulatively: {0,2,6,12,20}
        int cost = Mathf.Max(0, (numVotes - 1) * 2);

        OnVoteCalculation?.Invoke(ply, ref cost);

        return cost;
    }

    /// <summary>
    /// Calculates the cost of the roll
    /// </summary>
    /// <param name="ply">The player rolling</param>
    /// <param name="numRerolls">The number of rolls they have done so far</param>
    /// <returns></returns>
    public int CalculateRerollCost(CSteamID ply, int numRolls)
    {
        //Make sure the correct number of rerolls are free
        if (numRolls < FreeRolls) return 0;

        //Calculate how many rerolls we have used that aren't free.
        numRolls -= FreeRolls;

        //Formula. Can edit this however we like for balance.
        int cost = FirstRerollCost * (numRolls + 1);

        //Currently deciding to put this before roles tinker with it. Might change later, who knows.
        cost += GlobalRollCostMod;

        //Allow listeners to modify the cost
        OnRerollCalculation?.Invoke(ply, ref cost);

        return Mathf.Max(cost, 0);
    }

    /// <summary>
    /// Calculates the cost of the draw
    /// </summary>
    /// <param name="ply">The player drawing a card</param>
    /// <param name="numRerolls">The number of cards they have drawn so far</param>
    /// <returns></returns>
    public int CalculateDrawCost(ulong ply, int numDraws)
    {
        //Make sure the correct number of rerolls are free
        if (numDraws < FreeDraws) return 0;

        //Calculate how many rerolls we have used that aren't free.
        numDraws -= FreeDraws;

        //Formula. Can edit this however we like for balance.
        int cost = FirstRerollCost * (numDraws + 1);

        //Currently deciding to put this before roles tinker with it. Might change later, who knows.
        cost += GlobalDrawCostMod;

        //Allow listeners to modify the cost
        OnDrawCalculation?.Invoke(ply, ref cost);

        return Mathf.Max(cost, 0);
    }
}
