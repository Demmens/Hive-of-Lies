using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Remove plot point from set", menuName = "Missions/Effects/Specific/Remove Plot Point from Set")]
public class RemovePlotPointFromSet : MissionEffect
{
    [SerializeField] EMissionPlotPoint plotPoint;
    [SerializeField] MissionPlotPointSet set;

    public override void TriggerEffect()
    {
        set.Remove(plotPoint);
    }
}