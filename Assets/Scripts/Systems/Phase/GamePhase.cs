using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Represents a phase of the game, e.g. setup, mission vote, etc.
/// </summary>
public abstract class GamePhase : NetworkBehaviour
{
    /// <summary>
    /// Private counterpart of <see cref="Active"/>
    /// </summary>
    private bool active;

    /// <summary>
    /// Whether this is the current game phase
    /// </summary>
    public bool Active
    {
        get
        {
            return active;
        }
    }

    [SerializeField] private EGamePhase phase;
    /// <summary>
    /// The phase of the game this class represents
    /// </summary>
    public EGamePhase Phase
    {
        get
        {
            return phase;
        }
    }

    public event System.Action OnGamePhaseEnd;

    /// <summary>
    /// Call to start this phase
    /// </summary>
    public void ChangePhase()
    {
        active = true;
        Begin();
    }

    /// <summary>
    /// Called when this game phase begins
    /// </summary>
    public abstract void Begin();

    /// <summary>
    /// Call to end the phase and move to the next phase
    /// </summary>
    public virtual void End(bool forced = false)
    {
        active = false;
        if (!forced)
        {
            OnGamePhaseEnd?.Invoke();
        }
    }
}