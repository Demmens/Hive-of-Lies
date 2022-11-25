using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelNextFail : MissionEffect
{
    public override EffectType Type => EffectType.Positive;

    public override void TriggerEffect()
    {
        GameInfo.singleton.CancelNextFail = true;
        EndEffect();
    }
}
