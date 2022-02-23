using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleAbilityData : MonoBehaviour
{
    #region Private Properties
    /// <summary>
    /// Private counterpart to cref="Name"/>
    /// </summary>
    [SerializeField] new string name;

    /// <summary>
    /// Private counterpart to <see cref="Description"/>
    /// </summary>
    [TextArea]
    [SerializeField] string description;

    /// <summary>
    /// Private counterpart to <see cref="CanGiveTwoBees"/>
    /// </summary>
    [SerializeField] bool canGiveTwoBees = true;

    /// <summary>
    /// Whether this ability should also be instantiated on the client side.
    /// </summary>
    [SerializeField] bool instantiateOnClient;

    #endregion

    #region Public Properties
    /// <summary>
    /// The name of the ability
    /// </summary>
    public string Name
    {
        get
        {
            return name;
        }
    }

    /// <summary>
    /// The description of the ability
    /// </summary>
    public string Description
    {
        get
        {
            return description;
        }
    }

    /// <summary>
    /// Whether the ability can be given to the Two Bees in a Trench-Coat
    /// </summary>
    public bool CanGiveTwoBees
    {
        get
        {
            return canGiveTwoBees;
        }
    }
    #endregion
}
