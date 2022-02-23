using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for missions to only appear under specific circumstances
/// </summary>
public abstract class MissionCondition : MonoBehaviour
{
    /// <summary>
    /// The conditions under which the mission can appear
    /// </summary>
    /// <returns></returns>
    public abstract bool Condition();
}
