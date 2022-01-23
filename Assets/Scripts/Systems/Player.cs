using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    #region Private Properties
    /// <summary>
    /// Private version of <see cref="Influence"/>
    /// </summary>
    int influence = 0;

    /// <summary>
    /// Private version of <see cref="Abilities"/>
    /// </summary>
    List<RoleAbility> abilities;

    /// <summary>
    /// Private version of <see cref="Team"/>
    /// </summary>
    Team team = Team.Innocent;
    #endregion

    #region Public Properties
    /// <summary>
    /// The team that the player is on
    /// </summary>
    public Team Team
    {
        get
        {
            return team;
        }
        set
        {
            team = Team;
        }
    }

    /// <summary>
    /// The role abilities the player has
    /// </summary>
    public List<RoleAbility> Abilities
    {
        get
        {
            return abilities;
        }
        set
        {
            abilities = Abilities;
        }
    }

    /// <summary>
    /// The influence the player has
    /// </summary>
    public int Influence
    {
        get
        {
            return influence;
        }
        set
        {
            influence = Influence;
        }
    }

    #endregion

    public Player()
    {
        abilities = new List<RoleAbility>();
    }
}

/// <summary>
/// Innocent or Traitor. Currently no plans for more but may rename later.
/// </summary>
public enum Team
{
    Innocent,
    Traitor
}
