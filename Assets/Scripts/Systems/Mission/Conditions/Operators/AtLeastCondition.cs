using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "At Least", menuName = "Missions/Conditions/Operators/At Least")]
public class AtLeastCondition : MissionCondition
{
    [SerializeField] List<MissionCondition> conditions;
    [SerializeField] int num;

    public override bool Condition()
    {
        int numTrue = 0;
        foreach (MissionCondition cond in conditions)
        {
            if (cond.Condition()) if (++numTrue >= num) return true;
        }

        return false;
    }
}
