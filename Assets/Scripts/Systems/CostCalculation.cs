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
    /// Calculates the cost of the draw
    /// </summary>
    /// <param name="ply">The player drawing a card</param>
    /// <param name="numRerolls">The number of cards they have drawn so far</param>
    /// <returns></returns>
    [Server]
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
