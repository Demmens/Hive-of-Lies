using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedMissionsAnd : MissionCondition
{
    [SerializeField] private List<Mission> requiredMissions = new List<Mission>();
    [SerializeField] private MissionResult missionResult;

    public override bool Condition()
    {
        for (int i = 0; i < requiredMissions.Count; i++)
        {
            if (!GameInfo.CompletedMissions.TryGetValue(requiredMissions[i], out MissionResult result)) return false;
            if (result != missionResult) return false;
        }

        return true;
    }
}
