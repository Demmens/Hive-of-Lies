using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Completed Missions And", menuName = "Missions/Conditions/Completed Missions/Or")]
public class CompletedMissionsOr : MissionCondition
{
    [SerializeField] private List<MissionData> requiredMissions = new List<MissionData>();
    [SerializeField] private MissionResult missionResult;

    public override bool Condition()
    {
        for (int i = 0; i < requiredMissions.Count; i++)
        {
            foreach (KeyValuePair<Mission, MissionResult> pair in GameInfo.singleton.CompletedMissions) {
                if (pair.Key.Data != requiredMissions[i]) continue;

                if (pair.Value == missionResult) return true;
            }
        }

        return false;
    }
}
