using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class InfluencerAbility : RoleAbility
{
    [SerializeField] float drawCostMod;
    [SerializeField] float voteCostMod;
    void Start()
    {
        Owner.NextDrawCost.OnVariableChanged += ModifyDrawCost;
        Owner.NextVoteCost.OnVariableChanged += ModifyVoteCost;
    }

    void ModifyDrawCost(int oldVal, ref int newVal)
    {
        float change = newVal - oldVal;
        change *= drawCostMod;
        newVal = oldVal + Mathf.FloorToInt(change);
    }

    void ModifyVoteCost(int oldVal, ref int newVal)
    {
        float change = newVal - oldVal;
        change *= voteCostMod;
        newVal = oldVal + Mathf.FloorToInt(change);
    }
}
