using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealHoney : MissionEffect
{
    GameInfo info;
    /// <summary>
    /// How much to increase wasp research by
    /// </summary>
    [SerializeField] int honeyStolen;

    public override EffectType Type => honeyStolen > 0 ? EffectType.Negative : EffectType.Positive;

    void Start()
    {
        info = FindObjectOfType<GameInfo>();
    }
    public override void TriggerEffect()
    {
        info.HoneyStolen += honeyStolen;
        EndEffect();
    }
}
