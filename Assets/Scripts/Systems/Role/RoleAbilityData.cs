using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Roles/Create new ability")]
public class RoleAbilityData<T> : ScriptableObject where T : RoleAbility
{
    #region Private Properties
    /// <summary>
    /// Private version of <see cref="Name"/>
    /// </summary>
    [SerializeField] new string name;

    /// <summary>
    /// Private version of <see cref="Description"/>
    /// </summary>
    [TextArea]
    [SerializeField] string description;

    /// <summary>
    /// Private version of <see cref="CanGiveTwoBees"/>
    /// </summary>
    [SerializeField] bool canGiveTwoBees;

    /// <summary>
    /// Private version of <see cref="Ability"/>
    /// </summary>
    [SerializeReference] T ability;
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

    /// <summary>
    /// The script controlling the effect of the ability
    /// </summary>
    public RoleAbility Ability
    {
        get
        {
            return ability;
        }
        set
        {
            ability = Ability;
        }
    }
    #endregion
}
