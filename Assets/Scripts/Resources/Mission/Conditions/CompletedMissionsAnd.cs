using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedMissionsAnd : MissionCondition
{
    [SerializeField] private List<Mission> requiredMissions = new List<Mission>();

    public override bool Condition()
    {
        for (int i = 0; i < requiredMissions.Count; i++)
        {
            if (!GameInfo.CompletedMissions.Contains(requiredMissions[i])) return false;
        }

        return true;
    }
}
