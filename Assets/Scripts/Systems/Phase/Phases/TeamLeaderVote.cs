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

    [Tooltip("Set of all player votes")]
    [SerializeField] VoteSet allVotes;

    [Tooltip("Running total for the vote. If > 0, the team leader is voted in.")]
    [SerializeField] IntVariable voteTotal;

    [Tooltip("All players by their NetworkConnections")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("The number of players in the game.")]
    [SerializeField] IntVariable playerCount;



    [Tooltip("Invoked when the vote begins")]
    [SerializeField] GameEvent voteBegin;

    [Tooltip("Invoked when a player votes")]
    [SerializeField] GameEvent onPlayerVoted;

    [Tooltip("Invoked when all players have voted")]
    [SerializeField] GameEvent onAllPlayersVoted;

    public override void Begin()
    {
        allVotes.Value = new();
        currentVotes = new();
        voteTotal.Value = 0;
        playersClosedPopup = new();
        voteBegin?.Invoke();
    }

    [Server]
    public void PlayerIncreasedVote(NetworkConnection conn)
    {
        if (!Active) return;

        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        int cost = ply.NextUpvoteCost;        

        if (ply.Favour < cost && cost > 0) return;

        ply.NumVotes++;

        bool refund = ply.NumVotes < 0;

        ply.Favour.Value -= cost;

        int voteCheck = refund ? ply.NumVotes - 1 : ply.NumVotes;

        cost = CalculateVoteCost(voteCheck);
        if (!refund) cost *= -1;

        ply.NextDownvoteCost.Value = cost;

        cost = CalculateVoteCost(voteCheck + 1);
        if (refund) cost *= -1;

        ply.NextUpvoteCost.Value = cost;
    }

    [Server]
    public void PlayerDecreasedVote(NetworkConnection conn)
    {
        if (!Active) return;

        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        int cost = ply.NextDownvoteCost;

        if (ply.Favour < cost && cost > 0) return;

        ply.NumVotes--;

        bool refund = ply.NumVotes > 0;

        ply.Favour.Value -= cost;

        int voteCheck = refund ? ply.NumVotes + 1 : ply.NumVotes;

        cost = CalculateVoteCost(voteCheck);
        if (!refund) cost *= -1;

        ply.NextUpvoteCost.Value = cost;

        cost = CalculateVoteCost(voteCheck - 1);
        if (refund) cost *= -1;

        ply.NextDownvoteCost.Value = cost;
    }

    /// <summary>
    /// Call when a player locks in their vote
    /// </summary>
    /// <param name="ply">The player that voted</param>
    /// <param name="vote">How many votes the player sent</param>
    [Server]
    public void VoteLockedIn(NetworkConnection conn)
    {
        if (!Active) return;

        playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply);

        voteTotal.Value += ply.NumVotes;
        allVotes.Add(new PlayerVote()
        {
            ply = ply,
            votes = ply.NumVotes
        });

        //Invoke the player voted event
        onPlayerVoted?.Invoke();

        ply.NumVotes.Value = 0;
        ply.NextDownvoteCost.Value = 0;
        ply.NextUpvoteCost.Value = 0;

        //If we have received a vote from everyone
        if (allVotes.Value.Count == playerCount) AllVotesReceived();
    }

    void AllVotesReceived()
    {
        //Invoke the all players voted event
        onAllPlayersVoted?.Invoke();
        allVotes.Value = new();
    }

    /// <summary>
    /// Called when a player closes the vote result popup
    /// </summary>
    void VotePopupClosed(NetworkConnectionToClient conn)
    {
        playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply);
        if (playersClosedPopup.Contains(ply)) return;

        playersClosedPopup.Add(ply);

        bool lastPlayer = playersClosedPopup.Count == playerCount;


        if (lastPlayer)
        {
            AllPlayersClosedPopup();
        }
    }

    /// <summary>
    /// Called once all players have closed the vote result popup
    /// </summary>
    public void AllPlayersClosedPopup()
    {
        playersClosedPopup = new List<HoLPlayer>();
        allVotes.Value = new();
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

    /// <summary>
    /// Calculate the favour cost of the vote
    /// </summary>
    /// <param name="ply">The player who's voting</param>
    /// <param name="numVotes">The number of votes</param>
    public int CalculateVoteCost(int numVotes)
    {
        //Votes cost the same up and down
        numVotes = Mathf.Abs(numVotes);
        // Cost is 2(n-1) We can change this formula at any time for balance
        // Gives us the costs: {4,2,0,0,0,2,4}
        // Cumulatively: {0,2,6,12,20}
        int cost = Mathf.Max(0, (numVotes - 1) * 2);

        return cost;
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
    public HoLPlayer ply;
    /// <summary>
    /// How many votes the player sent
    /// </summary>
    public int votes; // int instead of bool in case we want to allow influence to be used for increasing number of votes.
}