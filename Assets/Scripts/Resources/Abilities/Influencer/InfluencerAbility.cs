using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class InfluencerAbility : RoleAbility
{
    CostCalculation costCalc;
    void Start()
    {
        costCalc = FindObjectOfType<CostCalculation>();

        costCalc.OnRerollCalculation += ModifyRollCost;
        costCalc.OnVoteCalculation += ModifyVoteCost;
    }

    void ModifyRollCost(CSteamID ply, ref int cost)
    {
        if (ply == Owner.SteamID && Active)
            cost *= 2;
    }

    void ModifyVoteCost(CSteamID ply, ref int cost)
    {
        if (ply == Owner.SteamID && Active)
            cost /= 2;
    }
}
