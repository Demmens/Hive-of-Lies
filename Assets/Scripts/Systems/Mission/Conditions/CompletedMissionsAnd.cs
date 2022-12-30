using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Completed Missions And", menuName = "Missions/Conditions/Completed Missions/And")]
public class CompletedMissionsAnd : MissionCondition
{
    [SerializeField] private List<Mission> requiredMissions = new();
    [SerializeField] MissionSet missionSet;

    public override bool Condition()
    {
        foreach (Mission miss in requiredMissions)
        {
            if (!missionSet.Value.Contains(miss)) return false;
        }

        return true;
    }
}