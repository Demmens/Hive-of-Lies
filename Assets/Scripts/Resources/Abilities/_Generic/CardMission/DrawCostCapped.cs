using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class DrawCostCapped : RoleAbility
{
    [SerializeField] int maxCost = 2;
    protected override void OnRoleGiven()
    {
        Owner.NextDrawCost.OnVariableChanged += ModifyDrawCost;
    }

    void ModifyDrawCost(int oldCost, ref int newCost)
    {
        newCost = Mathf.Min(newCost, maxCost);
    }
}
