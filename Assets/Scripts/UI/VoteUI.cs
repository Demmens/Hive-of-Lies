using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;

public class VoteUI : MonoBehaviour
{
    [SerializeField] TMP_Text voteNumber;
    [SerializeField] TMP_Text yesCost;
    [SerializeField] TMP_Text noCost;

    [SerializeField] GameObject yesVote;
    [SerializeField] GameObject noVote;
 
    [SerializeField] GameObject voteUI;
    [SerializeField] FavourController favourController;
    [SerializeField] CostCalculation costCalc;

    int _numVotes = 0;
    int _upVoteCost = 0;
    int _downVoteCost = 0;

    /// <summary>
    /// The cost of the next upvote
    /// </summary>
    int upVoteCost
    {
        get
        {
            return _upVoteCost;
        }
        set
        {
            _upVoteCost = value;
            yesCost.text = $"{-_upVoteCost}f";
        }
    }

    /// <summary>
    /// The cost of the next downvote
    /// </summary>
    int downVoteCost
    {
        get
        {
            return _downVoteCost;
        }
        set
        {
            _downVoteCost = value;
            noCost.text = $"{-_downVoteCost}f";
        }
    }

    /// <summary>
    /// The number of votes the client has currently placed
    /// </summary>
    int numVotes
    {
        get
        {
            return _numVotes;
        }
        set
        {
            _numVotes = value;
            RecalculateCost();
            voteNumber.text = numVotes.ToString();
        }
    }

    void Start()
    {
        NetworkClient.RegisterHandler<TeamLeaderVoteStartedMsg>(VoteStarted);
    }

    /// <summary>
    /// Called when the vote starts
    /// </summary>
    /// <param name="msg"></param>
    public void VoteStarted(TeamLeaderVoteStartedMsg msg)
    {
        voteUI.SetActive(true);
        numVotes = 0;
    }

    /// <summary>
    /// Called when a player increases their vote (upvotes)
    /// </summary>
    public void IncreaseVote()
    {
        numVotes++;

        noVote.SetActive(true);
        //if (upVoteCost > favourController.Favour) yesVote.SetActive(false);

        NetworkClient.Send(new PlayerChangeVoteMsg() {increased = true});
    }

    /// <summary>
    /// Called when a player deceases their vote (downvotes)
    /// </summary>
    public void DecreaseVote()
    {
        //favourController.Favour -= downVoteCost;
        numVotes--;

        yesVote.SetActive(true);
        //if (downVoteCost > favourController.Favour) noVote.SetActive(false);

        NetworkClient.Send(new PlayerChangeVoteMsg() { increased = false });
    }

    /// <summary>
    /// Recalculates the cost of increasing or decreasing your vote
    /// </summary>
    void RecalculateCost()
    {
        if (numVotes > 0)
        {
            upVoteCost = costCalc.CalculateVoteCost(numVotes + 1);
            downVoteCost = costCalc.CalculateVoteCost(numVotes);

            downVoteCost *= -1;
        }
        else
        {
            upVoteCost = costCalc.CalculateVoteCost(numVotes);
            downVoteCost = costCalc.CalculateVoteCost(numVotes - 1);

            upVoteCost *= -1;
        }
    }

    public void LockInVote()
    {
        voteUI.SetActive(false);
        NetworkClient.Send(new PlayerLockInMsg() { });
    }
}
