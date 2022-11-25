using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cancel Next Fail", menuName = "Missions/Effects/Cancel Next Fail")]
public class CancelNextFail : MissionEffect
{
    public override EffectType Type => EffectType.Positive;

    public override void TriggerEffect()
    {
        GameInfo.singleton.CancelNextFail = true;
        EndEffect();
    }
}
