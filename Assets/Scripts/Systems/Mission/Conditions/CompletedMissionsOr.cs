using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Completed Missions And", menuName = "Missions/Conditions/Completed Missions/Or")]
public class CompletedMissionsOr : MissionCondition
{
    [SerializeField] private List<Mission> requiredMissions = new();
    [SerializeField] private MissionSet missionSet;

    public override bool Condition()
    {
        foreach (Mission miss in missionSet.Value) {
            if (requiredMissions.Contains(miss)) return true;
        }

        return false;
    }
}
