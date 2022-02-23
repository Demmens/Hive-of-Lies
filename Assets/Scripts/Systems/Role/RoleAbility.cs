using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A powerful ability given to players, typically based on their role
/// </summary>
public class RoleAbility : MonoBehaviour
{

    #region Fields
    /// <summary>
    /// Private counterpart to <see cref="Enabled"/>
    /// </summary>
    bool active = true;

    #endregion

    #region Properties
    /// <summary>
    /// Whether this ability should have an effect
    /// </summary>
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = Active;
        }
    }

    /// <summary>
    /// The player that owns this ability
    /// </summary>
    public Player Owner;
    #endregion
}
