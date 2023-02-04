using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyDrawCost : RoleAbility
{
    [SerializeField] float drawCostMod;

    protected override void OnRoleGiven()
    {
        Owner.NextDrawCost.OnVariableChanged += OnDrawChange;
    }

    void OnDrawChange(int oldVal, ref int newVal)
    {
        newVal = Mathf.FloorToInt(newVal * drawCostMod);
    }
}
