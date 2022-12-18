using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RunMission : GamePhase
{
    public static RunMission singleton;

    /// <summary>
    /// The specific type of mission we want to run (e.g. cards, dice, etc.)
    /// </summary>
    [SerializeField] MissionType mission;

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

    [Tooltip("The currently active mission")]
    [SerializeField] MissionVariable currentMission;

    [Tooltip("Set of all completed missions")]
    [SerializeField] MissionSet completedMissions;

    [Tooltip("Set of all succeeded missions")]
    [SerializeField] MissionSet succeededMissions;

    [Tooltip("Set of all failed missions")]
    [SerializeField] MissionSet failedMissions;

    [Tooltip("All players on the mission")]
    [SerializeField] HoLPlayerSet playersOnMission;

    [Tooltip("All players")]
    [SerializeField] HoLPlayerSet players;

    private void Start()
    {
        //mission.OnMissionEnded += OnMissionEnded;
        singleton = this;
    }
    public override void Begin()
    {
        mission.Active = true;
        mission.StartMission();
    }

    void OnMissionEnded(MissionResult result, bool triggerEffects = true)
    {
        if (result == MissionResult.Success)
        {
            succeededMissions.Add(currentMission.Value);
        } 
        else 
        {
            failedMissions.Add(currentMission.Value);
        }
        completedMissions.Add(currentMission.Value);
        mission.Active = false;
        NetworkServer.SendToAll(new MissionEndMsg()
        {
            result = result,
        });

        foreach (HoLPlayer ply in playersOnMission.Value)
        {
            ply.Exhaustion++;
        }

        foreach (HoLPlayer ply in players.Value)
        {
            if (playersOnMission.Value.Contains(ply))
            {
                ply.Exhaustion++;
            }
            else
            {
                ply.Exhaustion.Value = 0;
            }
        }

        OnMissionEnd?.Invoke(result);

        if (triggerEffects)
        {
            List<MissionEffect> effects = (result == MissionResult.Success) ?
                currentMission.Value.SuccessEffects :
                currentMission.Value.FailEffects;

            numEffects = effects.Count;
            effectsTriggered = 0;
            //Trigger all effects
            foreach (MissionEffect effect in effects)
            {
                effect.OnMissionEffectFinished += OnEffectEnded;
                effect.TriggerEffect();
            }
        }
        else
        {
            End();
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