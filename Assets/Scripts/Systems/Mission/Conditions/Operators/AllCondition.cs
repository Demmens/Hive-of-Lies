using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All", menuName = "Missions/Conditions/Operators/All")]
public class AllCondition : MissionCondition
{
    [SerializeField] List<MissionCondition> conditions;

    public override bool Condition()
    {
        foreach (MissionCondition cond in conditions)
        {
            if (!cond.Condition()) return false;
        }

        return true;
    }
}
