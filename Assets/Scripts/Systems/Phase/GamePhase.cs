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

    /// <summary>
    /// Empty Delegate
    /// </summary>
    public delegate void BasicDelegate();
    /// <summary>
    /// Invoked when this game phases ends
    /// </summary>
    public event BasicDelegate OnGamePhaseEnd;
    /// <summary>
    /// Invoked when this game phase starts
    /// </summary>
    public event BasicDelegate OnGamePhaseStart;

    /// <summary>
    /// Call to start this phase
    /// </summary>
    public void ChangePhase()
    {
        active = true;
        GameInfo.singleton.GamePhase = Phase;
        OnGamePhaseStart?.Invoke();
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