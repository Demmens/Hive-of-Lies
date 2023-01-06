using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RunMission : GamePhase
{
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

    [Tooltip("Set of all players on the mission")]
    [SerializeField] HoLPlayerSet playersOnMission;

    [Tooltip("Set of all players")]
    [SerializeField] HoLPlayerSet players;

    [Tooltip("The result of the mission")]
    [SerializeField] MissionResultVariable missionResult;

    [Tooltip("Whether the next missions success effect should be cancelled")]
    [SerializeField] BoolVariable cancelNextSuccess;

    [Tooltip("Whether the next missions fail effect should be cancelled")]
    [SerializeField] BoolVariable cancelNextFail;

    [Tooltip("Invoked when the mission begins")]
    [SerializeField] GameEvent missionStarted;

    [Tooltip("Invoked when the mission end")]
    [SerializeField] GameEvent missionEnded;

    public override void Begin()
    {
        mission.Active = true;
        mission.StartMission();
        missionStarted?.Invoke();
    }

    public void EndMission()
    {
        Debug.Log("Mission should be ending now");
        if (missionResult == MissionResult.Success)
        {
            succeededMissions.Add(currentMission.Value);
        } 
        else 
        {
            failedMissions.Add(currentMission.Value);
        }
        completedMissions.Add(currentMission.Value);
        mission.Active = false;

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

        missionEnded?.Invoke();

        List<MissionEffect> effects = (missionResult == MissionResult.Success) ?
            currentMission.Value.SuccessEffects :
            currentMission.Value.FailEffects;

        numEffects = effects.Count;

        //Early out if there are no mission effects.
        if (numEffects == 0) End();

        effectsTriggered = 0;
        //Trigger all effects
        foreach (MissionEffect effect in effects)
        {
            effect.OnMissionEffectFinished += OnEffectEnded;
            effect.TriggerEffect();
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