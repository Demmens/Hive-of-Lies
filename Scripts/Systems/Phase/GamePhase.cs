using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a phase of the game, e.g. setup, mission vote, etc.
/// </summary>
public abstract class GamePhase : MonoBehaviour
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
    /// <summary>
    /// The phase of the game this class represents
    /// </summary>
    public abstract EGamePhase Phase
    {
        get;
    }

    /// <summary>
    /// Delegate for <see cref="OnGamePhaseChange"/>
    /// </summary>
    public delegate void GamePhaseChange();
    /// <summary>
    /// Invoked when this game phases ends
    /// </summary>
    public event GamePhaseChange OnGamePhaseChange;

    /// <summary>
    /// Call to start this phase
    /// </summary>
    public void ChangePhase()
    {
        active = true;
        GameInfo.GamePhase = Phase;
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
            OnGamePhaseChange?.Invoke();
        }
    }
}

/// <summary>
/// The phases of the game
/// </summary>
public enum EGamePhase
{
    Setup,
    DecideMission,
    DecideGeneral,
    GeneralPickPartners,
    GeneralVote,
    RunMission
}