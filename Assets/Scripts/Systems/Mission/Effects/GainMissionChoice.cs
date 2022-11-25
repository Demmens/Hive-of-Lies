using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gain Mission Choice", menuName = "Missions/Effects/Gain Mission Choice")]
public class GainMissionChoice : MissionEffect
{
    [SerializeField] int missionChoicesGained;
    public override EffectType Type => missionChoicesGained > 0 ? EffectType.Positive : EffectType.Negative;

    public override void TriggerEffect()
    {
        GameInfo.singleton.MissionChoices += missionChoicesGained;
        EndEffect();
    }
}
