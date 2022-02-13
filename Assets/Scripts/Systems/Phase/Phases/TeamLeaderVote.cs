using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLeaderVote : GamePhase
{
    #region Fields

    /// <summary>
    /// List of all player votes
    /// <para></para>
    /// Private counterpart to <see cref="Votes"/>
    /// </summary>
    List<PlayerVote> votes;

    /// <summary>
    /// Running total for the vote. If > 0, the TeamLeader is voted in.
    /// <para></para>
    /// Private counterpart to <see cref="VoteTotal"/>
    /// </summary>
    int voteTotal;

    #endregion

    #region Properties

    public override EGamePhase Phase
    {
        get
        {
            return EGamePhase.GeneralVote;
        }
    }

    /// <summary>
    /// List of all player votes
    /// </summary>
    public List<PlayerVote> Votes
    {
        get
        {
            return votes;
        }
        set
        {
            votes = value;
        }
    }

    /// <summary>
    /// Running total for the vote. If > 0, the team leader is voted in.
    /// </summary>
    public int VoteTotal
    {
        get
        {
            return voteTotal;
        }
        set
        {
            voteTotal = value;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Delegate for <see cref="OnPlayerVoted"/>
    /// </summary>
    /// <param name="ply">The player that voted</param>
    /// <param name="vote">How much they voted for</param>
    public delegate void PlayerVoted(Player ply, int vote);
    /// <summary>
    /// Invoked when a player votes
    /// </summary>
    public event PlayerVoted OnPlayerVoted;

    /// <summary>
    /// Delegate for <see cref="OnAllPlayersVoted"/>
    /// </summary>
    public delegate void AllPlayersVoted();
    /// <summary>
    /// Invoked when all players have voted
    /// </summary>
    public event AllPlayersVoted OnAllPlayersVoted;

    /// <summary>
    /// Delegate for <see cref="OnVoteCalculation"/>
    /// </summary>
    /// <param name="ply">The player voting</param>
    /// <param name="cost">The base cost of the vote</param>
    public delegate void VoteCalculation(Player ply, ref int cost);
    /// <summary>
    /// Invoked when the cost of voting is calculated
    /// </summary>
    public event VoteCalculation OnVoteCalculation;

    #endregion

    public override void Begin()
    {
        votes = new List<PlayerVote>();
        voteTotal = 0;
    }

    public void ChangedVoteNumber(Player ply, int oldNum, int newNum)
    {
        int cost = CalculateNextVoteCost(ply, newNum);

        //Don't remove favour if we can't afford it
        if (cost > ply.Favour) return;

        //If we've removed a vote, refund the cost, otherwise pay it.
        ply.Favour += Mathf.Abs(newNum) < Mathf.Abs(oldNum) ? cost : -cost;

        //Send the client the new information.
        //CalculateNextVoteCost(newNum - 1);
        //CalculateNextVoteCost(newNum + 1);
    }

    /// <summary>
    /// Call when a player locks in their vote
    /// </summary>
    /// <param name="ply">The player that voted</param>
    /// <param name="vote">How many votes the player sent</param>
    public void VoteLockedIn(Player ply, int vote)
    {
        if (!Active) return;

        voteTotal += vote;
        votes.Add(new PlayerVote()
        {
            ply = ply,
            votes = vote
        });

        //Invoke the player voted event
        OnPlayerVoted?.Invoke(ply, vote);

        //If we have received a vote from everyone
        if (votes.Count == GameInfo.PlayerCount) AllVotesReceived();
    }

    void AllVotesReceived()
    {
        //Invoke the all players voted event
        OnAllPlayersVoted?.Invoke();
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
    int CalculateNextVoteCost(Player ply, int numVotes)
    {
        //Votes cost the same up and down
        numVotes = Mathf.Abs(numVotes);
        // Cost is 2n We can change this formula at any time for balance
        // Gives us the costs: {0,2,4,6,8}
        // Cumulatively: {0,2,6,12,20}
        int cost = numVotes*2;

        OnVoteCalculation?.Invoke(ply, ref cost);

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
    public Player ply;
    /// <summary>
    /// How many votes the player sent
    /// </summary>
    public int votes; // int instead of bool in case we want to allow influence to be used for increasing number of votes.
}