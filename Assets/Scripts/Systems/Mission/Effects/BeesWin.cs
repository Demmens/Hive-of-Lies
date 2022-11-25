using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeesWin : MissionEffect
{
    GameEnd end;

    public override EffectType Type => EffectType.Positive;
    private void Start()
    {
        end = FindObjectOfType<GameEnd>();
    }

    public override void TriggerEffect()
    {
        end.EndGame(Team.Bee);
        EndEffect();
    }
}
