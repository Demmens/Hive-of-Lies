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

    public delegate void MissionEnded(MissionResult result, bool triggerEffects = true);
    public event MissionEnded OnMissionEnded;

    [SerializeField] IntVariable playerCount;
    [SerializeField] BoolVariable cancelNextSuccess;
    [SerializeField] BoolVariable cancelNextFail;

    protected virtual void Start()
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
    /// <param name="res">The result of the mission</param>
    /// <param name="triggerEffects">Whether the mission should trigger the success or fail effect</param>
    protected void EndMission(MissionResult res, bool triggerEffects = true)
    {
        if (!triggerEffects) 
        {
            OnMissionEnded?.Invoke(result, triggerEffects);
            return;
        }

        if (cancelNextFail && res == MissionResult.Fail || cancelNextSuccess && res == MissionResult.Success)
        {
            cancelNextFail.Value = false;
            cancelNextSuccess.Value = false;
            OnMissionEnded?.Invoke(res, false);
        }        
    }

    void PlayerClosedPopup(NetworkConnection conn, ClosedMissionResultPopupMsg msg)
    {
        if (popupsClosed.Contains(conn)) return;

        popupsClosed.Add(conn);
        if (popupsClosed.Count == playerCount) EndMission(result);
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
