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

    [Tooltip("The currently active mission")]
    [SerializeField] MissionVariable currentMission;

    [Tooltip("Set of all completed missions")]
    [SerializeField] MissionSet completedMissions;

    [Tooltip("Set of all players on the mission")]
    [SerializeField] HoLPlayerSet playersOnMission;

    [Tooltip("Set of all players")]
    [SerializeField] HoLPlayerSet players;

    [SerializeField] HoLPlayerVariable teamLeader;

    [SerializeField] IntVariable playerCount;

    [Tooltip("The result of the mission")]
    [SerializeField] MissionResultVariable missionResult;

    [Tooltip("The total of all played cards")]
    [SerializeField] IntVariable cardsTotal;

    [Tooltip("The total of all played cards")]
    [SerializeField] IntVariable missionDifficultyMod;

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
        completedMissions.Add(currentMission.Value);
        mission.Active = false;

        teamLeader.Value.Exhaustion++;
        foreach (HoLPlayer ply in players.Value)
        {
            if (ShouldExhaust(ply))
            {
                ply.Exhaustion++;
            }
            else
            {
                ply.Exhaustion.Value = 0;
            }
            ply.NumDraws.Value = 0;
            ply.NextDrawCost.Value = 0;
        }


        missionEnded?.Invoke();

        currentMission.Value.AfterAllEffectsTriggered += OnEffectEnded;
        //Trigger all effects
        currentMission.Value.TriggerValidEffects(cardsTotal - missionDifficultyMod);
    }

    bool ShouldExhaust(HoLPlayer ply)
    {
        //In fewer than 6 player games, only the team leader becomes exhausted
        if (playerCount < 6) return ply == teamLeader.Value;

        return playersOnMission.Value.Contains(ply) || ply == teamLeader;
    }

    private void OnEffectEnded()
    {
        Debug.Log("All mission effects have finished. Starting the next round");
        currentMission.Value.AfterAllEffectsTriggered -= OnEffectEnded;
        End();
    }
}