using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Information about the role
/// </summary>
[CreateAssetMenu(fileName = "RoleData", menuName = "Roles/Create role")]
public class RoleData : ScriptableObject
{
    #region Private Properties
    /// <summary>
    /// Private counterpart to <see cref="Name"/>
    /// </summary>
    [SerializeField] new string name;

    /// <summary>
    /// Private counterpart to <see cref="Description"/>
    /// </summary>
    [TextArea]
    [SerializeField] string description;

    /// <summary>
    /// Private counterpart to <see cref="Sprite"/>
    /// </summary>
    [SerializeField] Sprite sprite;

    /// <summary>
    /// Private counterpart to <see cref="StartingInfluence"/>
    /// </summary>
    [SerializeField] int startingInfluence;

    /// <summary>
    /// Private counterpart to <see cref="Team"/>
    /// </summary>
    [SerializeField] Team team;

    /// <summary>
    /// Private counterpart to <see cref="PlayersRequired"/>
    /// </summary>
    [SerializeField] int playersRequired;

    /// <summary>
    /// Prefab containing both RoleAbilityData and RoleAbility scripts
    /// </summary>
    [SerializeField] GameObject ability;
    #endregion

    #region Public Properties

    /// <summary>
    /// Name of the role
    /// </summary>
    public string Name
    {
        get
        {
            return name;
        }
    }

    /// <summary>
    /// Description for the role
    /// </summary>
    public string Description
    {
        get
        {
            return description;
        }
    }

    /// <summary>
    /// Sprite used for the role UI
    /// </summary>
    public Sprite Sprite
    {
        get
        {
            return sprite;
        }
    }

    /// <summary>
    /// Amount of influence the role starts with
    /// </summary>
    public int StartingInfluence
    {
        get
        {
            return startingInfluence;
        }
    }

    /// <summary>
    /// The team the role belongs to
    /// </summary>
    public Team Team
    {
        get
        {
            return team;
        }
    }

    /// <summary>
    /// The minimum number of players required in the lobby for this role to appear
    /// </summary>
    public int PlayersRequired
    {
        get
        {
            return playersRequired;
        }
    }

    /// <summary>
    /// The data for the roles ability
    /// </summary>
    public RoleAbilityData AbilityData
    {
        get
        {
            return ability.GetComponent<RoleAbilityData>();
        }
    }

    /// <summary>
    /// The logic for the roles ability
    /// </summary>
    public RoleAbility Ability
    {
        get
        {
            return ability.GetComponent<RoleAbility>();
        }
    }

    #endregion
}
