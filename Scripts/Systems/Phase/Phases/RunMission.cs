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

    [SerializeField] MissionType MissionType;

    public override void Begin()
    {
        MissionType.OnMissionEnded += new MissionType.MissionEnded(OnMissionEnded);
        MissionType.StartMission();

    }

    void OnMissionEnded()
    {
        End();
    }
}
