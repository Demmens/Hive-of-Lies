using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cancel Next Success", menuName = "Missions/Effects/Cancel Next Success")]
public class CancelNextSuccess : MissionEffect
{
    public override EffectType Type => EffectType.Negative;

    public override void TriggerEffect()
    {
        GameInfo.singleton.CancelNextSuccess = true;
        EndEffect();
    }
}
