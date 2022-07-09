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

    public delegate void MissionEnded(MissionResult result);
    /// <summary>
    /// Invoked when the mission ends.
    /// </summary>
    public event MissionEnded OnMissionEnd;

    /// <summary>
    /// The total number of mission effects that are going to trigger
    /// </summary>
    private int numEffects;
    /// <summary>
    /// How many of the mission effects have triggered so far
    /// </summary>
    private int effectsTriggered;

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

    void OnMissionEnded(MissionResult result, bool triggerEffects = true)
    {
        GameInfo.singleton.CompletedMissions.Add(GameInfo.singleton.CurrentMission, result);
        missionType.Active = false;
        NetworkServer.SendToAll(new MissionEndMsg()
        {
            result = result,
        });

        foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.singleton.Players)
        {
            if (GameInfo.singleton.PlayersOnMission.Contains(pair.Value))
            {
                pair.Value.Exhaustion++;
            }
            else
            {
                pair.Value.Exhaustion = 0;
            }
        }

        OnMissionEnd?.Invoke(result);

        if (triggerEffects)
        {
            List<MissionEffect> effects = (result == MissionResult.Fail) ?
                GameInfo.singleton.CurrentMission.SuccessEffects :
                GameInfo.singleton.CurrentMission.FailEffects;

            numEffects = effects.Count;
            effectsTriggered = 0;
            //Trigger all success effects
            foreach (MissionEffect effect in effects)
            {
                effect.OnMissionEffectFinished += OnEffectEnded;
                effect.TriggerEffect();
            }
        }
    }

    private void OnEffectEnded()
    {
        effectsTriggered++;
        if (effectsTriggered >= numEffects)
        {
            End();
        }
    }
}

public struct MissionEndMsg : NetworkMessage
{
    public MissionResult result;
}