using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMissionDifficulty : MissionEffect
{
    [SerializeField] int difficultyIncrease;
    public override void TriggerEffect()
    {
        DiceMission.TotalNeeded += difficultyIncrease;
    }
}
