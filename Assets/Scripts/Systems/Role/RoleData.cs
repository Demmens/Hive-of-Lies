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
    /// Private counterpart to <see cref="RoleName"/>
    /// </summary>
    [SerializeField] string roleName;

    /// <summary>
    /// Private counterpart to <see cref="Description"/>
    /// </summary>
    [TextArea]
    [SerializeField] string description;

    /// <summary>
    /// Private counterpart to <see cref="Sprite"/>
    /// </summary>
    [SerializeField] string sprite;

    /// <summary>
    /// Private counterpart to <see cref="StartingFavour"/>
    /// </summary>
    [SerializeField] int startingFavour;

    /// <summary>
    /// Private counterpart to <see cref="StartingDeck"/>
    /// </summary>
    [SerializeField] List<int> startingDeck = new List<int>
    {
        1,2,2,2,3,3,3,3,3,3,4,4,4,4,4,4,5,5,5,6
    };

    /// <summary>
    /// Private counterpart to <see cref="Team"/>
    /// </summary>
    [SerializeField] Team team;

    /// <summary>
    /// Private counterpart to <see cref="PlayersRequired"/>
    /// </summary>
    [SerializeField] int playersRequired;

    /// <summary>
    /// Prefab containing RoleAbility scripts
    /// </summary>
    [SerializeField] List<GameObject> abilities;

    /// <summary>
    /// Private counterpart of <see cref="Enabled"/>
    /// </summary>
    [SerializeField] bool enabled = true;
    #endregion

    #region Public Properties

    /// <summary>
    /// Name of the role
    /// </summary>
    public string RoleName
    {
        get
        {
            return roleName;
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
    public string Sprite
    {
        get
        {
            return sprite;
        }
    }

    /// <summary>
    /// Amount of influence the role starts with
    /// </summary>
    public int StartingFavour
    {
        get
        {
            return startingFavour;
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
    /// The logic for the roles ability
    /// </summary>
    public List<GameObject> Abilities
    {
        get
        {
            return abilities;
        }
    }

    /// <summary>
    /// Whether the role should appear in games
    /// </summary>
    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = value;
        }
    }

    /// <summary>
    /// The roles starting deck of cards
    /// </summary>
    public List<int> StartingDeck
    {
        get
        {
            return startingDeck;
        }
        set
        {
            startingDeck = value;
        }
    }
    #endregion
}
