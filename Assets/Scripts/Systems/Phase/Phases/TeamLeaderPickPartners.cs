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
    public override EGamePhase Phase
    {
        get
        {
            return EGamePhase.GeneralPickPartners;
        }
    }
    /// <summary>
    /// Number of players that go on the mission with the TeamLeader for each player count
    /// </summary>
    readonly Dictionary<int, int> partnerPlayerCounts = new Dictionary<int, int>()
    {
        {0,1},
        {6,2},
        {9,3}
    };

    /// <summary>
    /// Number of players that go on the mission with the TeamLeader in this game.
    /// </summary>
    public static int NumPartners;

    /// <summary>
    /// List of players the TeamLeader has selected so far
    /// </summary>
    List<Player> playersSelected;

    void Start()
    {
        //Find the appropriate number of players that need to go on each mission.
        for (int i = 0; i <= GameInfo.PlayerCount; i++)
        {
            if (partnerPlayerCounts.TryGetValue(i, out int num)) NumPartners = num;
        }

        NetworkServer.RegisterHandler<TeamLeaderChangePartnersMsg>(TeamLeaderSelectedPlayer);
        NetworkServer.RegisterHandler<TeamLeaderLockInMsg>(LockInChoices);
    }

    public override void Begin()
    {
        playersSelected = new List<Player>();
    }

    /// <summary>
    /// Called when the TeamLeader chooses a player to join them
    /// </summary>
    /// <param name="ply">The player that the TeamLeader has selected</param>
    void TeamLeaderSelectedPlayer(NetworkConnection conn, TeamLeaderChangePartnersMsg msg)
    {
        if (!Active) return;
        GameInfo.Players.TryGetValue(conn, out Player ply);
        if (ply != GameInfo.TeamLeader) return;

        Player target = null;
        foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.Players)
        {
            if (pair.Value.SteamID == msg.playerID)
            {
                target = pair.Value;
            }
        }

        if (target == null) return;

        if (msg.selected)
        {
            if (playersSelected.Count < NumPartners)
            {
                Debug.Log($"{SteamFriends.GetFriendPersonaName(ply.SteamID)} has selected {SteamFriends.GetFriendPersonaName(msg.playerID)}");
                playersSelected.Add(target);
            }
        }
        else
        {
            if (playersSelected.Contains(target))
                playersSelected.Remove(target);
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
        GameInfo.Players.TryGetValue(conn, out Player ply);
        if (ply != GameInfo.TeamLeader) return;

        Debug.Log("Team leader has locked in their partner choices");

        End();
    }
}

public struct TeamLeaderChangePartnersMsg : NetworkMessage
{
    public CSteamID playerID;
    public bool selected;
}
public struct TeamLeaderLockInMsg : NetworkMessage {}
