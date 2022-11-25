using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gain Research", menuName = "Missions/Effects/Gain Research")]
public class GainWaspResearch : MissionEffect
{
    GameInfo info;
    /// <summary>
    /// How much to increase wasp research by
    /// </summary>
    [SerializeField] int researchGain;

    public override EffectType Type => researchGain > 0 ? EffectType.Positive : EffectType.Negative;
    void Start()
    {
        info = FindObjectOfType<GameInfo>();
    }
    public override void TriggerEffect()
    {
        info.WaspResearch += researchGain;
        EndEffect();
    }
}
