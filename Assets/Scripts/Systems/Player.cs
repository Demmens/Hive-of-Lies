using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Private Properties
    /// <summary>
    /// Private counterpart to <see cref="Favour"/>
    /// </summary>
    int favour = 0;

    /// <summary>
    /// Private counterpart to <see cref="Team"/>
    /// </summary>
    Team team = Team.Bee;

    /// <summary>
    /// Private counterpart to <see cref="ID"/>
    /// </summary>
    int id;

    /// <summary>
    /// Private counterpart to <see cref="DisplayName"/>
    /// </summary>
    string displayName;
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
            team = value;
        }
    }

    /// <summary>
    /// The favour the player has
    /// </summary>
    public int Favour
    {
        get
        {
            return favour;
        }
        set
        {
            favour = value;
        }
    }

    /// <summary>
    /// The unique ID of the player
    /// </summary>
    public int ID
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }

    /// <summary>
    /// The display name of the player
    /// </summary>
    public string DisplayName
    {
        get
        {
            return displayName;
        }
        set
        {
            displayName = value;
        }
    }
    #endregion
}

/// <summary>
/// Innocent or Traitor. Currently no plans for more but may rename later.
/// </summary>
public enum Team
{
    Bee,
    Wasp
}
