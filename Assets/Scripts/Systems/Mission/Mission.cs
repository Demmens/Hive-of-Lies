using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Contains all the information about a Mission
/// </summary>
public class Mission
{
    /// <summary>
    /// The basic information about the mission
    /// </summary>
    public MissionData Data;

    /// <summary>
    /// Effects that happen upon a mission success
    /// </summary>
    public List<MissionEffect> SuccessEffects;

    /// <summary>
    /// Effects that happen upon a mission fail
    /// </summary>
    public List<MissionEffect> FailEffects;

    /// <summary>
    /// The condition that must be met for the mission to appear
    /// </summary>
    public System.Func<bool> Condition;

    /// <summary>
    /// Reference to the condition object we created, which we save for deletion later.
    /// </summary>
    MissionCondition conditionObject;

    public Mission(MissionData data, bool conditionOnly = false)
    {
        Data = data;
        
        if (conditionOnly)
        {
            //If the mission data has no condition, don't create an object
            conditionObject = data.Condition;
            //If the mission data has no condition, then the condition is always true
            if (conditionObject == null)
            {
                Condition = () => { return true; };
            }
            else
            {
                Condition = conditionObject.Condition;
            }
        }
        else
        {
            SuccessEffects = new List<MissionEffect>();
            FailEffects = new List<MissionEffect>();
        }
    }

    /// <summary>
    /// Call when finished with the mission to clean up the scene
    /// </summary>
    public void Destroy()
    {

    }
}
