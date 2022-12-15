using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "Missions/Create mission")]
public class Mission : ScriptableObject
{
    #region Fields
    /// <summary>
    /// Private counterpart to <see cref="MissionName"/>
    /// </summary>
    [SerializeField] string missionName;

    /// <summary>
    /// Private counterpart to <see cref="Description"/>
    /// </summary>
    [TextArea]
    [SerializeField] string description;

    /// <summary>
    /// Private counterpart to <see cref="FavourCost"/>
    /// </summary>
    [SerializeField] int influenceCost;

    [Space]
    [Space]

    /// <summary>
    /// Private counterpart to <see cref="SuccessFlavour"/>
    /// </summary>
    [TextArea]
    [SerializeField] string successFlavour;

    /// <summary>
    /// Private counterpart to <see cref="SuccessEffect"/>
    /// </summary>
    [SerializeField] List<MissionEffect> successEffects;

    [Space]
    [Space]

    /// <summary>
    /// Private counterpart to <see cref="FailFlavour"/>
    /// </summary>
    [TextArea]
    [SerializeField] string failFlavour;

    /// <summary>
    /// Private counterpart to <see cref="FailEffect"/>
    /// </summary>
    [SerializeField] List<MissionEffect> failEffects;

    [Space]
    [Space]

    /// <summary>
    /// Private counterpart to <see cref="Condition"/>
    /// </summary>
    [SerializeField] MissionCondition condition;
    #endregion

    #region Properties

    /// <summary>
    /// Name of the mission
    /// </summary>
    public string MissionName
    {
        get
        {
            return missionName;
        }
    }

    /// <summary>
    /// The flavour text for the mission
    /// </summary>
    public string Description
    {
        get
        {
            return description;
        }
    }
    /// <summary>
    /// How much it costs to stand for the teamLeader of this mission
    /// </summary>
    public int FavourCost
    {
        get
        {
            return influenceCost;
        }
    }

    /// <summary>
    /// What happens on a mission success
    /// </summary>
    public List<MissionEffect> SuccessEffects
    {
        get
        {
            return successEffects;
        }
    }

    /// <summary>
    /// The flavour text for the success effect
    /// </summary>
    public string SuccessFlavour
    {
        get
        {
            return successFlavour;
        }
    }

    /// <summary>
    /// What happens on a mission fail
    /// </summary>
    public List<MissionEffect> FailEffects
    {
        get
        {
            return failEffects;
        }
    }

    /// <summary>
    /// The flavour text for the fail effect
    /// </summary>
    public string FailFlavour
    {
        get
        {
            return failFlavour;
        }
    }

    /// <summary>
    /// The condition under which this mission will appear
    /// </summary>
    public MissionCondition Condition
    {
        get
        {
            return condition;
        }
    }
    #endregion
}