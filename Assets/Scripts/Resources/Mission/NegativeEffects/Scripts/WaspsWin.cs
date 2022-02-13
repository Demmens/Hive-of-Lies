using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspsWin : MissionEffect
{
    GameEnd end;

    private void Start()
    {
        end = FindObjectOfType<GameEnd>();
    }
    public override void TriggerEffect()
    {
        end.EndGame(Team.Wasp);
    }
}
