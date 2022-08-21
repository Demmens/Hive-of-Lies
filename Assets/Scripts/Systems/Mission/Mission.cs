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
    GameObject conditionObject;

    public Mission(MissionData data, bool conditionOnly = false)
    {
        Data = data;
        
        if (conditionOnly)
        {
            //If the mission data has no condition, don't create an object
            conditionObject = data.Condition == null ? null : Object.Instantiate(data.Condition);
            //If the mission data has no condition, then the condition is always true
            if (conditionObject == null)
            {
                Condition = () => { return true; };
            }
            else
            {
                Condition = conditionObject.GetComponent<MissionCondition>().Condition;
            }
        }
        else
        {
            SuccessEffects = new List<MissionEffect>();
            FailEffects = new List<MissionEffect>();

            foreach (MissionEffect effect in Data.SuccessEffects)
            {
                SuccessEffects.Add(Object.Instantiate(effect));
                //Spawn on all clients so we can do some client shenanigans with this as well.
                NetworkServer.Spawn(effect.gameObject);
            }
            foreach (MissionEffect effect in Data.FailEffects)
            {
                FailEffects.Add(Object.Instantiate(effect));
                //Spawn on all clients so we can do some client shenanigans with this as well.
                NetworkServer.Spawn(effect.gameObject);
            }
        }
    }

    /// <summary>
    /// Call when finished with the mission to clean up the scene
    /// </summary>
    public void Destroy()
    {
        if (SuccessEffects != null)
            foreach (MissionEffect effect in SuccessEffects)
                Object.Destroy(effect.gameObject);

        if (FailEffects != null)
            foreach (MissionEffect effect in FailEffects)
                Object.Destroy(effect.gameObject);

        if (conditionObject != null)
            Object.Destroy(conditionObject);
    }
}
