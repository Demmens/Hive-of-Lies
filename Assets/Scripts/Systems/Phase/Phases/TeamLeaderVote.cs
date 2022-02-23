using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    /// <summary>
    /// How many votes each player has currently placed
    /// </summary>
    Dictionary<Player, int> currentVotes;

    [SerializeField] CostCalculation costCalc;

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

    #endregion

    void Start()
    {
        NetworkServer.RegisterHandler<PlayerChangeVoteMsg>(ChangedVoteNumber);
        NetworkServer.RegisterHandler<PlayerLockInMsg>(VoteLockedIn);
    }

    public override void Begin()
    {
        votes = new List<PlayerVote>();
        voteTotal = 0;
    }

    void ChangedVoteNumber(NetworkConnection conn, PlayerChangeVoteMsg msg)
    {
        if (!Active) return;

        GameInfo.Players.TryGetValue(conn, out Player ply);

        currentVotes.TryGetValue(ply, out int votes);

        //Don't need to worry about 0 since our first vote is always free anyway.
        bool isPositive = votes > 0;

        votes += msg.increased ? 1 : -1;
        
        int cost = costCalc.CalculateVoteCost(ply, votes);

        //If we've removed a vote, refund the cost, otherwise pay it.
        if (msg.increased == isPositive) cost *= -1;

        //Don't remove favour if we can't afford it
        if (cost > ply.Favour) return;
        
        ply.Favour -= cost;

        currentVotes.Add(ply, votes);

        //Send the client the new information.
        //CalculateNextVoteCost(newNum - 1);
        //CalculateNextVoteCost(newNum + 1);
    }

    /// <summary>
    /// Call when a player locks in their vote
    /// </summary>
    /// <param name="ply">The player that voted</param>
    /// <param name="vote">How many votes the player sent</param>
    void VoteLockedIn(NetworkConnection conn, PlayerLockInMsg msg)
    {
        if (!Active) return;

        GameInfo.Players.TryGetValue(conn, out Player ply);

        currentVotes.TryGetValue(ply, out int vote);

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

public struct PlayerChangeVoteMsg : NetworkMessage
{
    public bool increased;
}

public struct PlayerLockInMsg : NetworkMessage
{

}