using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Mission(MissionData data)
    {
        Data = data;
        
        //If the mission data has no condition, don't create an object
        conditionObject = data.Condition == null ? null : Object.Instantiate(data.Condition);
        //If the mission data has no condition, then the condition is always true
        Condition = conditionObject == null ? (System.Func<bool>) (() => { return true; }) : conditionObject.Condition;
    }

    /// <summary>
    /// Call to create the success and fail effect objects
    /// </summary>
    public void CreateEffectObjects()
    {
        foreach (MissionEffect effect in Data.SuccessEffects)
        {
            SuccessEffects.Add(Object.Instantiate(effect));
        }
        foreach (MissionEffect effect in Data.FailEffects)
        {
            FailEffects.Add(Object.Instantiate(effect));
        }

        //If we're creating the effects, the condition is no longer required
        if (conditionObject != null)
            Object.Destroy(conditionObject);
    }

    /// <summary>
    /// Call when finished with the mission to clean up the scene
    /// </summary>
    public void Destroy()
    {
        foreach (MissionEffect effect in SuccessEffects)
            Object.Destroy(effect);

        foreach (MissionEffect effect in FailEffects)
            Object.Destroy(effect);

        if (conditionObject != null)
            Object.Destroy(conditionObject);
    }
}
