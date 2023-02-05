using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "None", menuName = "Missions/Conditions/Operators/None")]
public class NoneCondition : MissionCondition
{
    [SerializeField] List<MissionCondition> conditions;

    public override bool Condition()
    {
        foreach (MissionCondition cond in conditions)
        {
            if (cond.Condition()) return false;
        }

        return true;
    }
}
