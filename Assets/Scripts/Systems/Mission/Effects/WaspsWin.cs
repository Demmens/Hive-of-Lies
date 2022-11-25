using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspsWin : MissionEffect
{
    GameEnd end;
    public override EffectType Type => EffectType.Negative;

    private void Start()
    {
        end = FindObjectOfType<GameEnd>();
    }
    public override void TriggerEffect()
    {
        end.EndGame(Team.Wasp);
        EndEffect();
    }
}
