using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

[CreateAssetMenu(fileName = "Influencer Ability", menuName = "Roles/Abilities/Bee/Influencer")]
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
        cost *= 2;
    }

    void ModifyVoteCost(CSteamID ply, ref int cost)
    {
        cost /= 2;
    }
}
