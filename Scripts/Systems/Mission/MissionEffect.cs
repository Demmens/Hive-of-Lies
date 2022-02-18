using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionEffect : MonoBehaviour
{
    /// <summary>
    /// Private counterpart to <see cref="Description"/>
    /// </summary>
    [SerializeField] string description;

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
}
