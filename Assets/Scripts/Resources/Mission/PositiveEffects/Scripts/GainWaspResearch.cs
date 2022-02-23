using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainWaspResearch : MissionEffect
{
    GameInfo info;
    /// <summary>
    /// How much to increase wasp research by
    /// </summary>
    [SerializeField] int researchGain;
    void Start()
    {
        info = FindObjectOfType<GameInfo>();
    }
    public override void TriggerEffect()
    {
        info.WaspResearch += researchGain;
    }
}
