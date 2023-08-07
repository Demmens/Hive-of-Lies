using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Steamworks;

public class TeamLeaderVote : GamePhase
{
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

    [Tooltip("Invoked when there are more downvotes than upvotes")]
    [SerializeField] GameEvent voteFailed;

    [Tooltip("The UI associated with this game phase")]
    [SerializeField] VoteUI UI;

    private void Start()
    {
        playerCount.AfterVariableChanged += (val) => { if (allVotes.Value.Count == playerCount) AllVotesReceived(); };
    }

    public override void Begin()
    {
        allVotes.Value = new();
        voteTotal.Value = 0;
        voteBegin?.Invoke();
    }

    public void OnServerConnected(NetworkConnection conn)
    {
        if (!Active) return;
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        foreach (PlayerVote vote in allVotes.Value)
        {
            //If the player has already voted, we don't need to enable the UI.
            if (vote.ply == ply) return;
        }

        UI.TargetEnableUI(conn);
    }

    [Server]
    public void PlayerIncreasedVote(NetworkConnection conn)
    {
        if (!Active) return;

        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        int cost = ply.NextUpvoteCost;        

        if (ply.Favour < cost && cost > 0) return;

        ply.NumVotes++;

        ply.Favour.Value -= cost;

        ply.NextDownvoteCost.Value = CalculateDownvoteCost(ply.NumVotes);

        ply.NextUpvoteCost.Value = CalculateUpvoteCost(ply.NumVotes);
    }

    [Server]
    public void PlayerDecreasedVote(NetworkConnection conn)
    {
        if (!Active) return;

        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        int cost = ply.NextDownvoteCost;

        if (ply.Favour < cost && cost > 0) return;

        ply.NumVotes--;

        ply.Favour.Value -= cost;

        ply.NextUpvoteCost.Value = CalculateUpvoteCost(ply.NumVotes);

        ply.NextDownvoteCost.Value = CalculateDownvoteCost(ply.NumVotes);
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

        //If the vote was successful
        if (voteTotal > 0)
        {
            End();
        }
        else
        {
            //Back to standing for TeamLeader
            voteFailed?.Invoke();
        }
    }

    /// <summary>
    /// Calculate the costs of the next up vote
    /// </summary>
    /// <param name="numVotes"></param>
    /// <returns></returns>
    public static int CalculateUpvoteCost(int numVotes)
    {
        return numVotes >= 0 ? 2 * numVotes : 2 * (numVotes + 1);
    }

    /// <summary>
    /// Calculate the costs of the next down vote
    /// </summary>
    /// <param name="numVotes"></param>
    /// <returns></returns>
    public static int CalculateDownvoteCost(int numVotes)
    {
        return numVotes > 0 ? -2 * (numVotes-1) : -2 * numVotes;
    }
}

/// <summary>
/// Represents the vote of a player
/// </summary>
[System.Serializable]
public struct PlayerVote
{
    /// <summary>
    /// The player that this vote is from
    /// </summary>
    public HoLPlayer ply;
    /// <summary>
    /// How many votes the player sent
    /// </summary>
    public int votes;
}