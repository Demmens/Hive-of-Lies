using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class ModifyVoteCost : RoleAbility
{
    [SerializeField] float voteCostMod;
    protected override void OnRoleGiven()
    {
        Owner.NextUpvoteCost.OnVariableChanged += OnVoteChange;
        Owner.NextDownvoteCost.OnVariableChanged += OnVoteChange;
    }

    void OnVoteChange(int oldVal, ref int newVal)
    {
        newVal = Mathf.FloorToInt(newVal * voteCostMod);
    }
}
