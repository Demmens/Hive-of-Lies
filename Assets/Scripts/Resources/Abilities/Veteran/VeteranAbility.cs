using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class VeteranAbility : RoleAbility
{
    [SerializeField] int maxCost = 2;
    void Start()
    {
        Owner.NextDrawCost.OnVariableChanged += ModifyDrawCost;
    }

    void ModifyDrawCost(int oldCost, ref int newCost)
    {
        newCost = Mathf.Min(newCost, maxCost);
    }
}
