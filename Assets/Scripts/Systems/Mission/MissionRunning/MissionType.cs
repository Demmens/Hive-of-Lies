using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Contains information on how the mission will be played (with a deck of cards, rolling dice etc.)
/// </summary>
public abstract class MissionType : MonoBehaviour
{

    /// <summary>
    /// Whether the current phase of the game is the mission phase
    /// </summary>
    public bool Active;

    /// <summary>
    /// Who the current TeamLeader of the mission is
    /// </summary>
    protected Player General;

    /// <summary>
    /// List of connections that have closed the mission result popup
    /// </summary>
    List<NetworkConnection> popupsClosed;

    /// <summary>
    /// The result of the mission
    /// </summary>
    protected MissionResult result;

    /// <summary>
    /// Reference to the game information
    /// </summary>
    [SerializeField] protected GameInfo Info { get; private set; }

    public delegate void MissionEnded(MissionResult result);
    public event MissionEnded OnMissionEnded;

    void Start()
    {
        NetworkServer.RegisterHandler<ClosedMissionResultPopupMsg>(PlayerClosedPopup);
    }

    /// <summary>
    /// Called when the mission begins (after TeamLeader is successfully voted in)
    /// </summary>
    public virtual void StartMission()
    {
        popupsClosed = new List<NetworkConnection>();
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
                foreach (MissionEffect effect in GameInfo.CurrentMission.SuccessEffects)
                {
                    effect.TriggerEffect();
                }
            }
            else
            {
                //Trigger all fail effects
                foreach (MissionEffect effect in GameInfo.CurrentMission.FailEffects)
                {
                    effect.TriggerEffect();
                }
            }
        }
        
        OnMissionEnded?.Invoke(result);
    }

    void PlayerClosedPopup(NetworkConnection conn, ClosedMissionResultPopupMsg msg)
    {
        if (popupsClosed.Contains(conn)) return;

        popupsClosed.Add(conn);
        if (popupsClosed.Count == GameInfo.PlayerCount) EndMission(result);
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
