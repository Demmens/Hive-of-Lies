using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class CostCalculation : NetworkBehaviour
{
    [Tooltip("Number of draws the player gets to make for free. Should always be at least 1.")]
    [SerializeField] IntVariable freeDraws;

    /// <summary>
    /// Number times the player may draw a card for free. Should always be at least 1.
    /// </summary>
    [SyncVar] int FreeDraws = 2;

    [Tooltip("Cost of the first paid draw.")]
    [SerializeField] IntVariable firstDrawCost;

    /// <summary>
    /// Cost of the first paid draw
    /// </summary>
    [SyncVar] int FirstDrawCost = 4;

    [Tooltip("How much to increase all draw costs by")]
    [SerializeField] IntVariable globalDrawCostMod;

    /// <summary>
    /// How much to increase all draw costs by
    /// </summary>
    [SyncVar] int GlobalDrawCostMod = 0;

    private void Start()
    {
        freeDraws.AfterVariableChanged += (int val) => { if (NetworkServer.active) FreeDraws = val; };
        firstDrawCost.AfterVariableChanged += (int val) => { if (NetworkServer.active) FirstDrawCost = val; };
        globalDrawCostMod.AfterVariableChanged += (int val) => { if (NetworkServer.active) GlobalDrawCostMod = val; };
    }

    /// <summary>
    /// Calculate the favour cost of the vote
    /// </summary>
    /// <param name="ply">The player who's voting</param>
    /// <param name="numVotes">The number of votes</param>
    public int CalculateVoteCost(int numVotes)
    {
        //Votes cost the same up and down
        numVotes = Mathf.Abs(numVotes);
        // Cost is 2n-1 We can change this formula at any time for balance
        // Gives us the costs: {0,0,2,4,6,8}
        // Cumulatively: {0,2,6,12,20}
        int cost = Mathf.Max(0, (numVotes - 1) * 2);

        return cost;
    }

    /// <summary>
    /// Calculates the cost of the draw
    /// </summary>
    /// <param name="ply">The player drawing a card</param>
    /// <param name="numRerolls">The number of cards they have drawn so far</param>
    /// <returns></returns>
    public int CalculateDrawCost(int numDraws)
    {
        //Make sure the correct number of rerolls are free
        if (numDraws < FreeDraws) return 0;

        //Calculate how many rerolls we have used that aren't free.
        numDraws -= FreeDraws;

        //Formula. Can edit this however we like for balance.
        int cost = FirstDrawCost * (numDraws + 1);

        //Currently deciding to put this before roles tinker with it. Might change later, who knows.
        cost += GlobalDrawCostMod;

        return Mathf.Max(cost, 0);
    }
}
