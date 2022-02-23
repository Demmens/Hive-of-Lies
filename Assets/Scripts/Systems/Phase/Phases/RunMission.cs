using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunMission : GamePhase
{
    public override EGamePhase Phase
    {
        get
        {
            return EGamePhase.RunMission;
        }
    }

    [SerializeField] MissionType missionType;

    public override void Begin()
    {
        missionType.OnMissionEnded += new MissionType.MissionEnded(OnMissionEnded);
        missionType.Active = true;
        missionType.StartMission();

    }

    void OnMissionEnded()
    {
        missionType.Active = false;
        End();
    }
}
