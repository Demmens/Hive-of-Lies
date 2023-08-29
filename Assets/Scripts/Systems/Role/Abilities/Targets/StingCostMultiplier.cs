using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StingCostMultiplier : RoleAbility
{
    [SerializeField] float multiplier;
    protected override void OnRoleGiven()
    {
        Owner.StingCost.Value = Mathf.RoundToInt(Owner.StingCost.Value * multiplier);
    }
}
