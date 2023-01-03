using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Greater Than", menuName = "Missions/Conditions/Variable Comparison/IntVar >")]
public class IntVariableGreaterThan : MissionCondition
{
    [SerializeField] IntVariable variable;
    [SerializeField] int num;

    public override bool Condition()
    {
        return variable > num;
    }
}
