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

    /// <summary>
    /// Invoked when the Team Leader locks in their partner choices
    /// </summary>
    public event LockInPartners OnLockInChoices;
    public delegate void LockInPartners();

    void Start()
    {
        //Find the appropriate number of players that need to go on each mission.
        for (int i = 0; i <= playerCount; i++)
        {
            if (partnerPlayerCounts.TryGetValue(i, out int num)) numPartners.Value = num;
        }
        NetworkServer.RegisterHandler<TeamLeaderChangePartnersMsg>(TeamLeaderSelectedPlayer);
        NetworkServer.RegisterHandler<TeamLeaderLockInMsg>(LockInChoices);
    }

    public override void Begin()
    {
        playersSelected.Value = new List<HoLPlayer>();
        NetworkServer.SendToAll(new TeamLeaderStartPickingMsg() { teamLeaderID = teamLeader.Value.PlayerID });
    }

    /// <summary>
    /// Called when the TeamLeader chooses a player to join them
    /// </summary>
    /// <param name="ply">The player that the TeamLeader has selected</param>
    void TeamLeaderSelectedPlayer(NetworkConnection conn, TeamLeaderChangePartnersMsg msg)
    {
        if (!Active) return;
        playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply);
        if (ply != teamLeader) return;

        HoLPlayer target = null;
        foreach (HoLPlayer ply2 in players.Value)
        {
            if (ply2.PlayerID == (ulong) msg.playerID)
            {
                target = ply2;
            }
        }

        if (target == null) return;

        if (msg.selected)
        {
            if (playersSelected.Value.Count < numPartners)
            {
                Debug.Log($"{ply.DisplayName} has selected {SteamFriends.GetFriendPersonaName(msg.playerID)}");
                playersSelected.Add(target);
            }
        }
        else
        {
            if (playersSelected.Value.Contains(target)) playersSelected.Remove(target);
        }

        NetworkServer.SendToAll(new TeamLeaderChangePartnersMsg()
        {
            playerID = msg.playerID,
            selected = msg.selected
        });
    }

    /// <summary>
    /// Called when the TeamLeader locks in their choices of players for the mission.
    /// </summary>
    public void LockInChoices(NetworkConnection conn, TeamLeaderLockInMsg msg)
    {
        if (!Active) return;
        playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply);
        if (ply != teamLeader) return;

        playersOnMission.Value = playersSelected.Value;
        Debug.Log("Team leader has locked in their partner choices");
        OnLockInChoices?.Invoke();

        End();
    }
}

public struct TeamLeaderStartPickingMsg : NetworkMessage
{
    public ulong teamLeaderID;
}

public struct TeamLeaderChangePartnersMsg : NetworkMessage
{
    public CSteamID playerID;
    public bool selected;
}
public struct TeamLeaderLockInMsg : NetworkMessage { }
