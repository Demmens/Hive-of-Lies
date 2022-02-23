using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostCalculation : MonoBehaviour
{
    /// <summary>
    /// Number of rolls the player gets to make for free. Should always be at least 1.
    /// </summary>
    [SerializeField] int freeRolls = 2;
    /// <summary>
    /// Cost of the first paid reroll
    /// </summary>
    [SerializeField] int firstRerollCost = 2;

    /// <summary>
    /// Sends vote cost information out for modification
    /// </summary>
    public event VoteCalculation OnVoteCalculation;
    public delegate void VoteCalculation(Player ply, ref int cost);

    /// <summary>
    /// Sends roll cost information out for modification
    /// </summary>
    public event RerollCalculation OnRerollCalculation;
    public delegate void RerollCalculation(Player ply, ref int cost);


    /// <summary>
    /// Calculate the favour cost of the vote
    /// </summary>
    /// <param name="ply">The player who's voting</param>
    /// <param name="numVotes">The number of votes</param>
    public int CalculateVoteCost(Player ply, int numVotes)
    {
        //Votes cost the same up and down
        numVotes = Mathf.Abs(numVotes);
        // Cost is 2n-1 We can change this formula at any time for balance
        // Gives us the costs: {0,0,2,4,6,8}
        // Cumulatively: {0,2,6,12,20}
        int cost = Mathf.Max(0,(numVotes-1) * 2);

        OnVoteCalculation?.Invoke(ply, ref cost);

        return cost;
    }

    /// <summary>
    /// Calculates the cost of the roll
    /// </summary>
    /// <param name="ply">The player rolling</param>
    /// <param name="numRerolls">The number of rolls they have done so far</param>
    /// <returns></returns>
    public int CalculateRerollCost(Player ply, int numRolls)
    {
        //Make sure the correct number of rerolls are free
        if (numRolls < freeRolls) return 0;

        //Calculate how many rerolls we have used that aren't free.
        numRolls -= freeRolls;

        //Formula. Can edit this however we like for balance.
        int cost = firstRerollCost * (numRolls + 1);

        //Allow listeners to modify the cost
        OnRerollCalculation?.Invoke(ply, ref cost);

        return Mathf.Max(cost, 0);
    }
}
