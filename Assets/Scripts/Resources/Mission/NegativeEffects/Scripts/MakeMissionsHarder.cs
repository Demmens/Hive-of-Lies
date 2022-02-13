using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeMissionsHarder : MissionEffect
{
    [SerializeField] int difficultyIncrease;
    public override void TriggerEffect()
    {
        DiceMission.TotalNeeded += difficultyIncrease;
    }
}
