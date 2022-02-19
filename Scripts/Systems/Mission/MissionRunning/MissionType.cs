using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information on how the mission will be played (with a deck of cards, rolling dice etc.)
/// </summary>
public abstract class MissionType : MonoBehaviour
{
    /// <summary>
    /// Private counterpart to <see cref="Active"/>
    /// </summary>
    private bool active;

    /// <summary>
    /// Whether the current phase of the game is the mission phase
    /// </summary>
    public bool Active;

    /// <summary>
    /// Who the current TeamLeader of the mission is
    /// </summary>
    protected Player General;

    /// <summary>
    /// All players on the mission
    /// </summary>
    protected List<Player> Players;

    /// <summary>
    /// Reference to the game information
    /// </summary>
    [SerializeField] protected GameInfo Info { get; private set; }

    public delegate void MissionEnded();
    public event MissionEnded OnMissionEnded;

    /// <summary>
    /// Called when the mission begins (after TeamLeader is successfully voted in)
    /// </summary>
    public virtual void StartMission()
    {

    }

    /// <summary>
    /// Call to end the mission
    /// </summary>
    /// <param name="result">The result of the mission</param>
    /// <param name="triggerEffects">Whether the mission should trigger the success or fail effect</param>
    protected void EndMission(MissionResult result, bool triggerEffects = true)
    {
        if (triggerEffects)
        {
            if (result == MissionResult.Success)
            {
                //Trigger all success effects
                foreach (MissionEffect effect in Info.CurrentMission.SuccessEffects)
                {
                    effect.TriggerEffect();
                }
            }
            else
            {
                //Trigger all fail effects
                foreach (MissionEffect effect in Info.CurrentMission.FailEffects)
                {
                    effect.TriggerEffect();
                }
            }
        }
        
        OnMissionEnded?.Invoke();
    }
}

/// <summary>
/// Currently only success or fail, but maybe we'll want something new in future
/// </summary>
public enum MissionResult
{
    Success,
    Fail
}
