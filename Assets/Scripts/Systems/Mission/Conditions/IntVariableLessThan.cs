using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Less Than", menuName = "Missions/Conditions/Variable Comparison/IntVar <")]
public class IntVariableLessThan : MissionCondition
{
    [SerializeField] IntVariable variable;
    [SerializeField] int num;

    public override bool Condition()
    {
        return variable < num;
    }
}
