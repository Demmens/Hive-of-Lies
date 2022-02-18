using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluencerAbility : RoleAbility
{
    DiceMission dice;
    TeamLeaderVote vote;
    void Start()
    {
        dice = FindObjectOfType<DiceMission>();
        vote = FindObjectOfType<TeamLeaderVote>();

        dice.OnRerollCaculation += new DiceMission.RerollCalculation(ModifyRollCost);
        vote.OnVoteCalculation += new TeamLeaderVote.VoteCalculation(ModifyVoteCost);
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
