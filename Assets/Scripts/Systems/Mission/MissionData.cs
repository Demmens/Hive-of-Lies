using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Missions/Create mission")]
public class MissionData : ScriptableObject
{
    #region Private Properties
    /// <summary>
    /// Private counterpart to <see cref="Name"/>
    /// </summary>
    [SerializeField] new string name;

    /// <summary>
    /// Private counterpart to <see cref="InfluenceCost"/>
    /// </summary>
    [SerializeField] int influenceCost;

    /// <summary>
    /// Private counterpart to <see cref="SuccessEffect"/>
    /// </summary>
    [SerializeField] MissionEffect successEffect;

    /// <summary>
    /// Private counterpart to <see cref="FailEffect"/>
    /// </summary>
    [SerializeField] MissionEffect failEffect;
    #endregion

    #region Public Properties

    /// <summary>
    /// Name of the mission
    /// </summary>
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = Name;
        }
    }

    /// <summary>
    /// How much it costs to stand for the general of this mission
    /// </summary>
    public int InfluenceCost
    {
        get
        {
            return influenceCost;
        }
        set
        {
            influenceCost = InfluenceCost;
        }
    }

    /// <summary>
    /// What happens on a mission success
    /// </summary>
    public MissionEffect SuccessEffect
    {
        get
        {
            return successEffect;
        }
        set
        {
            successEffect = SuccessEffect;
        }
    }

    /// <summary>
    /// What happens on a mission fail
    /// </summary>
    public MissionEffect FailEffect
    {
        get
        {
            return failEffect;
        }
        set
        {
            failEffect = FailEffect;
        }
    }
    #endregion
}
