using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntVar Comparison", menuName = "Missions/Conditions/Variable Comparison/IntVar")]
public class IntVariableCondition : MissionCondition
{
    [SerializeField] IntVariable variable;
    [SerializeField] Comparator comparator;
    [SerializeField] int num;

    public override bool Condition()
    {
        switch (comparator)
        {
            case Comparator.GreaterThan: return variable > num;
            case Comparator.EqualTo: return variable == num;
            case Comparator.LessThan: return variable < num;
            default: return variable == num;
        }
    }
}
