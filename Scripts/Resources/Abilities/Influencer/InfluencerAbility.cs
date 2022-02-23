using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluencerAbility : RoleAbility
{
    CostCalculation costCalc;
    void Start()
    {
        costCalc = FindObjectOfType<CostCalculation>();

        costCalc.OnRerollCalculation += ModifyRollCost;
        costCalc.OnVoteCalculation += ModifyVoteCost;
    }

    void ModifyRollCost(Player ply, ref int cost)
    {
        if (ply == Owner && Active)
            cost *= 2;
    }

    void ModifyVoteCost(Player ply, ref int cost)
    {
        if (ply == Owner && Active)
            cost /= 2;
    }
}
