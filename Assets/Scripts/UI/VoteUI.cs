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

    int numVotes = 0;
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

    void Start()
    {
        NetworkClient.RegisterHandler<TeamLeaderVoteStartedMsg>(VoteStarted);
    }

    public void VoteStarted(TeamLeaderVoteStartedMsg msg)
    {
        voteUI.SetActive(true);
    }

    public void IncreaseVote()
    {
        numVotes++;
        voteNumber.text = numVotes.ToString();
        favourController.Favour -= upVoteCost;

        RecalculateCost();
        noVote.SetActive(true);
        if (upVoteCost > favourController.Favour) yesVote.SetActive(false);

        NetworkClient.Send(new PlayerChangeVoteMsg() {increased = true});
    }

    public void DecreaseVote()
    {
        numVotes--;
        voteNumber.text = numVotes.ToString();
        favourController.Favour -= downVoteCost;

        RecalculateCost();
        yesVote.SetActive(true);
        if (downVoteCost > favourController.Favour) noVote.SetActive(false);

        NetworkClient.Send(new PlayerChangeVoteMsg() { increased = false });
    }

    void RecalculateCost()
    {
        if (numVotes > 0)
        {
            upVoteCost = costCalc.CalculateVoteCost(SteamUser.GetSteamID(), numVotes + 1);
            downVoteCost = costCalc.CalculateVoteCost(SteamUser.GetSteamID(), numVotes);

            downVoteCost *= -1;
        }
        else
        {
            upVoteCost = costCalc.CalculateVoteCost(SteamUser.GetSteamID(), numVotes);
            downVoteCost = costCalc.CalculateVoteCost(SteamUser.GetSteamID(), numVotes - 1);

            upVoteCost *= -1;
        }
    }

    public void LockInVote()
    {
        voteUI.SetActive(false);
        NetworkClient.Send(new PlayerLockInMsg() { });
    }
}
