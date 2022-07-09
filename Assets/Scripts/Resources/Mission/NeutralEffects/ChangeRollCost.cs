using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRollCost : MissionEffect
{
    CostCalculation costCalc;
    [SerializeField] int rollCostChange;
    void Start()
    {
        costCalc = CostCalculation.singleton;
    }

    public override void TriggerEffect()
    {
        costCalc.GlobalRollCostMod += rollCostChange;
        EndEffect();
    }
}
