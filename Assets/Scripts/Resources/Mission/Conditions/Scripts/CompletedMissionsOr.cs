using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SucceededMissionsOr : MissionCondition
{
    [SerializeField] private List<Mission> requiredMissions = new List<Mission>();
    [SerializeField] private MissionResult missionResult;

    public override bool Condition()
    {
        for (int i = 0; i < requiredMissions.Count; i++)
        {
            if (!GameInfo.CompletedMissions.TryGetValue(requiredMissions[i], out MissionResult result)) continue;

            if (result == missionResult) return true;
        }

        return false;
    }
}
