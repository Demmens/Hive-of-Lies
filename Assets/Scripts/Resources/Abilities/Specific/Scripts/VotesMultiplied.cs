using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VotesMultiplied : RoleAbility
{
    [SerializeField] int multiplier;
    protected override void OnRoleGiven()
    {
        Owner.NumVotes.OnVariableChanged +=  NumVotesChanged;
    }

    void NumVotesChanged(int oldVal, ref int newVal)
    {
        //Reduced vote
        if (oldVal > newVal) newVal -= multiplier - 1;
        //Increased vote
        if (newVal > oldVal) newVal += multiplier - 1;

        int val = newVal;

        StartCoroutine(Coroutines.Delay(() =>
        {
            Owner.NextUpvoteCost.Value = TeamLeaderVote.CalculateUpvoteCost(val / multiplier);
            Owner.NextDownvoteCost.Value = TeamLeaderVote.CalculateDownvoteCost(val / multiplier);
        }));
    }
}
