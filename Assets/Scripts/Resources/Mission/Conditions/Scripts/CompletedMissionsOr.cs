using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedMissionsOr : MissionCondition
{
    [SerializeField] private List<MissionData> requiredMissions = new List<MissionData>();
    [SerializeField] private MissionResult missionResult;

    public override bool Condition()
    {
        for (int i = 0; i < requiredMissions.Count; i++)
        {
            foreach (KeyValuePair<Mission, MissionResult> pair in GameInfo.CompletedMissions) {
                if (pair.Key.Data != requiredMissions[i]) continue;

                if (pair.Value == missionResult) return true;
            }
        }

        return false;
    }
}
