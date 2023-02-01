using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// A powerful ability given to players, typically based on their role
/// </summary>
public class RoleAbility : NetworkBehaviour
{

    #region Fields
    /// <summary>
    /// Private counterpart to <see cref="Active"/>
    /// </summary>
    [HideInInspector]
    public bool active = true;

    /// <summary>
    /// The player that owns this ability
    /// </summary>
    [HideInInspector]
    public HoLPlayer Owner;

    #endregion
}
