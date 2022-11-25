using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Change Draw Cost", menuName = "Missions/Effects/Change Draw Cost")]
public class ChangeDrawCost : MissionEffect
{
    CostCalculation costCalc;
    [SerializeField] int drawCostChange;
    public override EffectType Type => drawCostChange > 0 ? EffectType.Negative : EffectType.Positive;
    void Start()
    {
        costCalc = CostCalculation.singleton;
    }

    public override void TriggerEffect()
    {
        costCalc.GlobalDrawCostMod += drawCostChange;
        EndEffect();
    }
}
