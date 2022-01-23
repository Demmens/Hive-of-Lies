using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Private Properties
    /// <summary>
    /// Private counterpart to <see cref="Influence"/>
    /// </summary>
    int influence = 0;

    /// <summary>
    /// Private counterpart to <see cref="Team"/>
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
}

/// <summary>
/// Innocent or Traitor. Currently no plans for more but may rename later.
/// </summary>
public enum Team
{
    Innocent,
    Traitor
}
