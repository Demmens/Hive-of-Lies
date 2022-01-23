using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A powerful ability given to players, typically based on their role
/// </summary>
public abstract class RoleAbility
{

    #region Private Properties

    /// <summary>
    /// Private version of <see cref="Enabled"/>
    /// </summary>
    bool enabled = true;

    #endregion

    #region Public Properties

    /// <summary>
    /// Whether this ability should have an effect
    /// </summary>
    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = Enabled;
        }
    }

    #endregion

}
