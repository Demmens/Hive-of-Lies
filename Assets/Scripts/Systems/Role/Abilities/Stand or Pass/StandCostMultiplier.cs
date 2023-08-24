using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandCostMultiplier : RoleAbility
{
    [SerializeField] float multiplier;

    void Start()
    {
        Owner.NextStandCost.OnVariableChanged += OnStandCostChanged;
    }

    void OnStandCostChanged(int oldVal, ref int newVal)
    {
        //Since the cost to stand is never added to or minused from, this is fine.
        //If this ability is ever buggy though, this is probably the reason.
        newVal = Mathf.RoundToInt(newVal * multiplier);
    }
}
