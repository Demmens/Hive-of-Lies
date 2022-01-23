using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A powerful ability given to players, typically based on their role
/// </summary>
public class RoleAbility : MonoBehaviour
{

    #region Private Properties
    /// <summary>
    /// Private counterpart to <see cref="Enabled"/>
    /// </summary>
    bool active = true;

    /// <summary>
    /// Private counterpart to <see cref="Owner"/>
    /// </summary>
    Player owner;

    /// <summary>
    /// Reference to the event system
    /// </summary>
    protected EventSystem events;
    #endregion

    #region Public Properties
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
    public Player Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = Owner;
        }
    }
    #endregion

    private void Start()
    {
        events = FindObjectOfType<EventSystem>();
    }
}
