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

    private void Start()
    {
        missionType.OnMissionEnded += OnMissionEnded;
    }
    public override void Begin()
    {
        missionType.Active = true;
        missionType.StartMission();
    }

    void OnMissionEnded()
    {
        missionType.Active = false;
        End();
    }
}