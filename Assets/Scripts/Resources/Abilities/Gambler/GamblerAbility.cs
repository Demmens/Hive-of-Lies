using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class GamblerAbility : RoleAbility
{
    DiceMission dice;
    CostCalculation costCalc;

    List<int> previousRolls;
    bool goneBust = false;

    void Start()
    {
        dice = FindObjectOfType<DiceMission>();
        costCalc = FindObjectOfType<CostCalculation>();

        dice.OnPlayerRolled += PlayerRolled;
        ClientEventProvider.singleton.OnPlayerRolled += PlayerRolled;

        costCalc.OnRerollCalculation += ModifyRerollCost;

        dice.OnMissionEnded += MissionEnd;
        ClientEventProvider.singleton.OnMissionEnd += MissionEnd;
    }

    /// <summary>
    /// If bust, the next reroll costs 99 favour
    /// </summary>
    /// <param name="ply">Player rerolling</param>
    /// <param name="cost">Cost of the reroll</param>
    void ModifyRerollCost(CSteamID ply, ref int cost)
    {
        if (ply != Owner.SteamID) return;

        cost = goneBust ? 99 : 0;
    }

    /// <summary>
    /// Client version of player rolled
    /// </summary>
    void PlayerRolled(PlayerRolledMsg msg)
    {
        CheckForBust(msg.rollResult);
    }

    /// <summary>
    /// Server version of player rolled
    /// </summary>
    /// <param name="ply">The player that rolled</param>
    /// <param name="roll">The roll information</param>
    void PlayerRolled(Player ply, ref PlayerRollInfo roll)
    {
        if (ply != Owner) return;
        CheckForBust(roll.currentRoll);
    }

    /// <summary>
    /// Check if the roll makes the player go bust
    /// </summary>
    /// <param name="roll">The new roll</param>
    void CheckForBust(int roll)
    {
        if (previousRolls.Contains(roll))
        {
            GoBust();
            return;
        }

        previousRolls.Add(roll);
    }

    /// <summary>
    /// Logic for what happens on going bust
    /// </summary>
    void GoBust()
    {
        goneBust = true;
    }

    /// <summary>
    /// Reset previous rolls on mission end
    /// </summary>
    void MissionEnd()
    {
        previousRolls = new List<int>();
        goneBust = false;
    }
}
