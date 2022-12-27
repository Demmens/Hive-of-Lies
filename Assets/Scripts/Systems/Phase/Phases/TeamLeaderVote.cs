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



    [Tooltip("Invoked when the vote begins")]
    [SerializeField] GameEvent voteBegin;

    [Tooltip("Invoked when a player votes")]
    [SerializeField] GameEvent onPlayerVoted;

    [Tooltip("Invoked when all players have voted")]
    [SerializeField] GameEvent onAllPlayersVoted;

    public override void Begin()
    {
        votes = new List<PlayerVote>();
        currentVotes = new Dictionary<HoLPlayer, int>();
        voteTotal.Value = 0;
        playersClosedPopup = new List<HoLPlayer>();
        voteBegin?.Invoke();
    }

    [Server]
    public void PlayerIncreasedVote(NetworkConnection conn)
    {
        ChangedVoteNumber(conn, true);
    }

    [Server]
    public void PlayerDecreasedVote(NetworkConnection conn)
    {
        ChangedVoteNumber(conn, false);
    }

    [Server]
    void ChangedVoteNumber(NetworkConnection conn, bool increased)
    {
        if (!Active) return;

        playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply);

        int cost = increased ? ply.NextUpvoteCost : ply.NextDownvoteCost;

        //Whether we are refunding our previous vote or not
        bool refund = (ply.NumVotes > 0) != increased;

        //If the player doesn't have enough favour to change their vote
        if (ply.Favour < cost && !refund) return;

        ply.Favour.Value -= cost;

        if (increased)
        {
            ply.NextDownvoteCost.Value = -ply.NextUpvoteCost;
        }
        else
        {
            ply.NextUpvoteCost.Value = -ply.NextDownvoteCost;
        }

        int refundIndex = refund ? ply.NumVotes : Mathf.Abs(ply.NumVotes) +1;
        cost = CalculateVoteCost(refundIndex);

        ply.NumVotes.Value += increased ? 1 : -1;

        if (exists) currentVotes[ply] = votes;
        else currentVotes.Add(ply, votes);
    }

    /// <summary>
    /// Call when a player locks in their vote
    /// </summary>
    /// <param name="ply">The player that voted</param>
    /// <param name="vote">How many votes the player sent</param>
    void VoteLockedIn(NetworkConnectionToClient conn = null)
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
        // Gives us the costs: {0,0,2,4,6,8}
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
    public ulong ply;
    /// <summary>
    /// How many votes the player sent
    /// </summary>
    public int votes; // int instead of bool in case we want to allow influence to be used for increasing number of votes.
}