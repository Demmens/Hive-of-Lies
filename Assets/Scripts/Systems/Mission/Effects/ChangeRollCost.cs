using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRollCost : MissionEffect
{
    CostCalculation costCalc;
    [SerializeField] int rollCostChange;
    public override EffectType Type => rollCostChange > 0 ? EffectType.Negative : EffectType.Positive;
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
