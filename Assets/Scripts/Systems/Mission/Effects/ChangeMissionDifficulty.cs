using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Change Difficulty", menuName = "Missions/Effects/Specific/Change Mission Difficulty")]
public class ChangeMissionDifficulty : MissionEffect
{
    [SerializeField] Mission mission;
    [SerializeField] int difficultyIncrease;

    public override void TriggerEffect()
    {
        mission.DifficultyMod += difficultyIncrease;
        EndEffect();
    }
}
