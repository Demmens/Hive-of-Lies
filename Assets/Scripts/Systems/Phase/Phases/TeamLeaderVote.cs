using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Steamworks;

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

    /// <summary>
    /// The players that have closed the vote popup
    /// </summary>
    List<Player> playersClosedPopup;

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
    /// Invoked when a player votes
    /// </summary>
    public UnityEvent<Player,int> OnPlayerVoted;

    /// <summary>
    /// Invoked when all players have voted
    /// </summary>
    public UnityEvent OnAllPlayersVoted;

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
        currentVotes = new Dictionary<Player, int>();
        voteTotal = 0;
        playersClosedPopup = new List<Player>();
        NetworkServer.SendToAll(new TeamLeaderVoteStartedMsg() { });
    }

    void ChangedVoteNumber(NetworkConnection conn, PlayerChangeVoteMsg msg)
    {
        if (!Active) return;
        GameInfo.singleton.Players.TryGetValue(conn, out Player ply);

        bool exists = currentVotes.TryGetValue(ply, out int votes);

        bool refund = (votes > 0) != msg.increased;

        int refundIndex = refund ? votes : Mathf.Abs(votes)+1;
        int cost = costCalc.CalculateVoteCost(ply.SteamID, refundIndex);

        votes += msg.increased ? 1 : -1;

        //If we've removed a vote, refund the cost, otherwise pay it.
        if (refund) cost *= -1;

        //Don't remove favour if we can't afford it
        if (cost > ply.Favour) return;
        
        ply.Favour -= cost;

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

        GameInfo.singleton.Players.TryGetValue(conn, out Player ply);

        currentVotes.TryGetValue(ply, out int vote);

        voteTotal += vote;
        votes.Add(new PlayerVote()
        {
            ply = ply.SteamID,
            votes = vote
        });

        //Invoke the player voted event
        OnPlayerVoted?.Invoke(ply, vote);

        //If we have received a vote from everyone
        if (votes.Count == GameInfo.singleton.PlayerCount) AllVotesReceived();
    }

    void AllVotesReceived()
    {
        //Invoke the all players voted event
        OnAllPlayersVoted?.Invoke();

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
        GameInfo.singleton.Players.TryGetValue(conn, out Player ply);
        if (playersClosedPopup.Contains(ply)) return;

        playersClosedPopup.Add(ply);

        bool lastPlayer = playersClosedPopup.Count == GameInfo.singleton.PlayerCount;

        NetworkServer.SendToAll(new VoteContinueClickedMsg()
        {
            closedBy = ply.SteamID,
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
        playersClosedPopup = new List<Player>();
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
    public CSteamID ply;
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
    public CSteamID closedBy;

    /// <summary>
    /// Whether this is the last player to close the popup
    /// </summary>
    public bool lastPlayer;
}