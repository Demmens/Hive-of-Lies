using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMissionDifficulty : MissionEffect
{
    [SerializeField] int difficultyIncrease;

    public override EffectType Type => difficultyIncrease > 0 ? EffectType.Negative : EffectType.Positive;
    public override void TriggerEffect()
    {
        CardsMission.Difficulty += difficultyIncrease;
        EndEffect();
    }
}
