using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Change Free Draws", menuName = "Missions/Effects/Change Free Draws")]
public class ChangeFreeDraws : MissionEffect
{
    CostCalculation costCalc;
    [SerializeField] int freeDrawChange;

    public override EffectType Type => freeDrawChange > 0 ? EffectType.Positive : EffectType.Negative;
    void Start()
    {
        costCalc = CostCalculation.singleton;
    }

    public override void TriggerEffect()
    {
        costCalc.FreeDraws += freeDrawChange;
        EndEffect();
    }
}
