using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFreeRolls : MissionEffect
{
    CostCalculation costCalc;
    [SerializeField] int freeRollChange;
    void Start()
    {
        costCalc = CostCalculation.singleton;
    }

    public override void TriggerEffect()
    {
        costCalc.FreeRolls += freeRollChange;
    }
}
