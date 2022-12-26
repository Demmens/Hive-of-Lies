using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

/// <summary>
/// Allows the TeamLeader to choose players to join them on the mission.
/// </summary>
public class TeamLeaderPickPartners : GamePhase
{
    /// <summary>
    /// Number of players that go on the mission with the TeamLeader for each player count
    /// </summary>
    readonly Dictionary<int, int> partnerPlayerCounts = new Dictionary<int, int>()
    {
        {0,1},
        {2,2},
        {6,3},
        {9,4}
    };

    [Tooltip("Number of players that go on the mission with the TeamLeader in this game.")]
    [SerializeField] IntVariable numPartners;

    [Tooltip("Number of players in the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

    [Tooltip("List of players the TeamLeader has selected so far")]
    [SerializeField] HoLPlayerSet playersSelected;

    [Tooltip("List of all players by their NetworkConnection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("List of all players")]
    [SerializeField] HoLPlayerSet players;

    [Tooltip("List of all players on the mission")]
    [SerializeField] HoLPlayerSet playersOnMission;

    [Tooltip("Invoked at the start of this game phase")]
    [SerializeField] GameEvent teamLeaderCanPick;

    [Tooltip("Invoked when the team leader has locked in their choices for partners")]
    [SerializeField] GameEvent partnerChoicesLocked;

    void Start()
    {
        //Find the appropriate number of players that need to go on each mission.
        for (int i = 0; i <= playerCount; i++)
        {
            if (partnerPlayerCounts.TryGetValue(i, out int num)) numPartners.Value = num;
        }
    }

    public override void Begin()
    {
        playersSelected.Value = new List<HoLPlayer>();
        teamLeaderCanPick?.Invoke();
    }

    /// <summary>
    /// Called when the TeamLeader chooses a player to join them
    /// </summary>
    /// <param name="conn">The connection of the player that the TeamLeader has selected</param>
    public void TeamLeaderSelectedPlayer(NetworkConnection conn)
    {
        if (!Active) return;
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        if (ply == teamLeader) return;

        if (!playersOnMission.Value.Contains(ply))
        {
            if (playersSelected.Value.Count < numPartners)
            {
                Debug.Log($"{teamLeader.Value.DisplayName} has selected {ply.DisplayName}");
                playersSelected.Add(ply);
            }
        }
        else
        {
            playersSelected.Remove(ply);
        }
    }

    /// <summary>
    /// Called when the TeamLeader locks in their choices of players for the mission.
    /// </summary>
    public void LockInChoices()
    {
        if (!Active) return;

        playersOnMission.Value = playersSelected.Value;
        Debug.Log("Team leader has locked in their partner choices");

        End();
    }
}