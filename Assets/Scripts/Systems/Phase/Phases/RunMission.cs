using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    void OnMissionEnded(MissionResult result)
    {
        missionType.Active = false;
        NetworkServer.SendToAll(new MissionEndMsg() { result = result });
        End();
    }
}

public struct MissionEndMsg : NetworkMessage
{
    public MissionResult result;
}