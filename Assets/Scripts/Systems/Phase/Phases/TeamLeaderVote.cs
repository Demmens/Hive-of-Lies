using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Steamworks;

public class TeamLeaderVote : GamePhase
{

    /// <summary>
    /// How many votes each player has currently placed
    /// </summary>
    Dictionary<HoLPlayer, int> currentVotes;

    /// <summary>
    /// The players that have closed the vote popup
    /// </summary>
    List<HoLPlayer> playersClosedPopup;

    [SerializeField] CostCalculation costCalc;

    /// <summary>
    /// List of all player votes
    /// </summary>
    public List<PlayerVote> votes;

    [Tooltip("Running total for the vote. If > 0, the team leader is voted in.")]
    [SerializeField] IntVariable voteTotal;

    [Tooltip("All players by their NetworkConnections")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("The number of players in the game.")]
    [SerializeField] IntVariable playerCount;

    #region Events

    [Tooltip("Invoked when a player votes")]
    [SerializeField] GameEvent onPlayerVoted;

    [Tooltip("Invoked when all players have voted")]
    [SerializeField] GameEvent onAllPlayersVoted;

    #endregion

    void Start()
    {
        NetworkServer.RegisterHandler<PlayerChangeVoteMsg>(ChangedVoteNumber);
        NetworkServer.RegisterHandler<PlayerLockInMsg>(VoteLockedIn);
        NetworkServer.RegisterHandler<VoteContinueClickedMsg>(VotePopupClosed);
    }

    public override void Begin()
    {
        votes = new List<PlayerVote>();
        currentVotes = new Dictionary<HoLPlayer, int>();
        voteTotal.Value = 0;
        playersClosedPopup = new List<HoLPlayer>();
        NetworkServer.SendToAll(new TeamLeaderVoteStartedMsg() { });
    }

    void ChangedVoteNumber(NetworkConnection conn, PlayerChangeVoteMsg msg)
    {
        if (!Active) return;
        playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply);

        bool exists = currentVotes.TryGetValue(ply, out int votes);

        bool refund = (votes > 0) != msg.increased;

        int refundIndex = refund ? votes : Mathf.Abs(votes)+1;
        int cost = costCalc.CalculateVoteCost(new CSteamID(ply.PlayerID), refundIndex);

        votes += msg.increased ? 1 : -1;

        //If we've removed a vote, refund the cost, otherwise pay it.
        if (refund) cost *= -1;

        //Don't remove favour if we can't afford it
        if (cost > ply.Favour) return;
        
        ply.Favour.Value -= cost;

        if (exists) currentVotes[ply] = votes;
        else currentVotes.Add(ply, votes);
    }

    /// <summary>
    /// Call when a player locks in their vote
    /// </summary>
    /// <param name="ply">The player that voted</param>
    /// <param name="vote">How many votes the player sent</param>
    void VoteLockedIn(NetworkConnection conn, PlayerLockInMsg msg)
    {
        if (!Active) return;

        playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply);

        currentVotes.TryGetValue(ply, out int vote);

        voteTotal.Value += vote;
        votes.Add(new PlayerVote()
        {
            ply = ply.PlayerID,
            votes = vote
        });

        //Invoke the player voted event
        onPlayerVoted?.Invoke();

        //If we have received a vote from everyone
        if (votes.Count == playerCount) AllVotesReceived();
    }

    void AllVotesReceived()
    {
        //Invoke the all players voted event
        onAllPlayersVoted?.Invoke();

        NetworkServer.SendToAll(new SendVoteResultMsg()
        {
            votes = votes
        });
    }

    /// <summary>
    /// Called when a player closes the vote result popup
    /// </summary>
    void VotePopupClosed(NetworkConnection conn, VoteContinueClickedMsg msg)
    {
        playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply);
        if (playersClosedPopup.Contains(ply)) return;

        playersClosedPopup.Add(ply);

        bool lastPlayer = playersClosedPopup.Count == playerCount;

        NetworkServer.SendToAll(new VoteContinueClickedMsg()
        {
            closedBy = ply.PlayerID,
            lastPlayer = lastPlayer
        });

        if (lastPlayer)
        {
            AllPlayersClosedPopup();
        }
    }

    /// <summary>
    /// Called once all players have closed the vote result popup
    /// </summary>
    void AllPlayersClosedPopup()
    {
        playersClosedPopup = new List<HoLPlayer>();
        votes = new List<PlayerVote>();
        //If the vote was successful
        if (voteTotal > 0)
        {
            End();
        }
        else
        {
            //Back to standing for TeamLeader
        }
    }
}

/// <summary>
/// Represents the vote of a player
/// </summary>
public struct PlayerVote
{
    /// <summary>
    /// The player that this vote is from
    /// </summary>
    public ulong ply;
    /// <summary>
    /// How many votes the player sent
    /// </summary>
    public int votes; // int instead of bool in case we want to allow influence to be used for increasing number of votes.
}

public struct TeamLeaderVoteStartedMsg : NetworkMessage
{

}

public struct PlayerChangeVoteMsg : NetworkMessage
{
    public bool increased;
}

public struct PlayerLockInMsg : NetworkMessage
{

}

public struct SendVoteResultMsg : NetworkMessage
{
    public List<PlayerVote> votes;
}

public struct VoteContinueClickedMsg : NetworkMessage
{
    /// <summary>
    /// The player that closed the popup
    /// </summary>
    public ulong closedBy;

    /// <summary>
    /// Whether this is the last player to close the popup
    /// </summary>
    public bool lastPlayer;
}