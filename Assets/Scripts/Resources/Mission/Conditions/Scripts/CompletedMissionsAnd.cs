using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedMissionsAnd : MissionCondition
{
    [SerializeField] private List<MissionData> requiredMissions = new List<MissionData>();
    [SerializeField] private MissionResult missionResult;

    public override bool Condition()
    {
        for (int i = 0; i < requiredMissions.Count; i++)
        {
            bool isMet = false;
            foreach (KeyValuePair<Mission, MissionResult> pair in GameInfo.singleton.CompletedMissions)
            {
                if (requiredMissions[i] != pair.Key.Data) continue;

                if (pair.Value != missionResult) return false;

                isMet = true;
            }

            if (isMet == false) return false;
        }

        return true;
    }
}
