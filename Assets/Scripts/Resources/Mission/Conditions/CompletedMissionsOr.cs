using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedMissionsOr : MissionCondition
{
    [SerializeField] private List<Mission> requiredMissions = new List<Mission>();

    public override bool Condition()
    {
        for (int i = 0; i < GameInfo.CompletedMissions.Count; i++)
        {
            if (requiredMissions.Contains(GameInfo.CompletedMissions[i])) return true;
        }

        return false;
    }
}
