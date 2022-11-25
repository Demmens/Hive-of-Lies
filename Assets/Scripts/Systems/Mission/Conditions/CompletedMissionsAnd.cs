using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Completed Missions And", menuName = "Missions/Conditions/Completed Missions/And")]
public class CompletedMissionsAnd : MissionCondition
{
    [SerializeField] private List<CompletedMission> requiredMissions = new List<CompletedMission>();

    public override bool Condition()
    {
        for (int i = 0; i < requiredMissions.Count; i++)
        {
            bool isMet = false;
            foreach (KeyValuePair<Mission, MissionResult> pair in GameInfo.singleton.CompletedMissions)
            {
                if (requiredMissions[i].mission != pair.Key) continue;

                if (!requiredMissions[i].ignoreResult && pair.Value != requiredMissions[i].result) return false;

                isMet = true;
            }

            if (isMet == false) return false;
        }

        return true;
    }

    [System.Serializable]
    private struct CompletedMission
    {
        public Mission mission;
        public bool ignoreResult;
        public MissionResult result;
    }
}