using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class VeteranAbility : RoleAbility
{
    [SerializeField] int maxCost = 2;
    CostCalculation costCalc;
    void Start()
    {
        costCalc = FindObjectOfType<CostCalculation>();
        costCalc.OnRerollCalculation += CalculateRerollCost;
    }

    void CalculateRerollCost(CSteamID id, ref int cost)
    {
        if (id == Owner.SteamID)
        {
            cost = Mathf.Min(cost, maxCost);
        }
    }
}
