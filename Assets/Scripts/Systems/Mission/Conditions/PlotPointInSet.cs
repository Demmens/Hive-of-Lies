using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Greater Than", menuName = "Missions/Conditions/Item in Set/Mission Plot Point")]
public class PlotPointInSet : MissionCondition
{
    [SerializeField] EMissionPlotPoint plotPoint;
    [SerializeField] MissionPlotPointSet set;

    public override bool Condition()
    {
        return set.Value.Contains(plotPoint);
    }
}
