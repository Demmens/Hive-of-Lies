using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionEffect : MonoBehaviour
{
    /// <summary>
    /// Private counterpart to <see cref="Description"/>
    /// </summary>
    [SerializeField] string description;

    public delegate void MissionEffectOver();
    /// <summary>
    /// Invoked when the mission effect has finished. Important for effects that aren't instantaneous (e.g. investigation)
    /// </summary>
    public event MissionEffectOver OnMissionEffectFinished;

    /// <summary>
    /// Description of the effect
    /// </summary>
    public string Description
    {
        get
        {
            return description;
        }
    }

    /// <summary>
    /// Trigger this mission effect
    /// </summary>
    public abstract void TriggerEffect();

    /// <summary>
    /// Call on the server to end the mission effect
    /// </summary>
    protected void EndEffect()
    {
        OnMissionEffectFinished?.Invoke();
    }
}
