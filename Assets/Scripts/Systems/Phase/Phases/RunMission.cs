using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RunMission : GamePhase
{
    public static RunMission singleton;
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
        singleton = this;
    }
    public override void Begin()
    {
        missionType.Active = true;
        missionType.StartMission();
    }

    void OnMissionEnded(MissionResult result)
    {
        GameInfo.CompletedMissions.Add(GameInfo.CurrentMission, result);
        missionType.Active = false;
        NetworkServer.SendToAll(new MissionEndMsg()
        {
            result = result,
        });

        foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.Players)
        {
            if (GameInfo.PlayersOnMission.Contains(pair.Value))
            {
                pair.Value.Exhaustion++;
            }
            else
            {
                pair.Value.Exhaustion = 0;
            }
        }

        End();
    }
}

public struct MissionEndMsg : NetworkMessage
{
    public MissionResult result;
}