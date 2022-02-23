using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VoteUI : MonoBehaviour
{
    [SerializeField] TMP_Text voteNumber;
    [SerializeField] TMP_Text yesCost;
    [SerializeField] TMP_Text noCost;

    [SerializeField] GameObject voteUI;
    [SerializeField] FavourController favourController;
    [SerializeField] CostCalculation costCalc;

    int numVotes = 0;
    int upVoteCost = 0;
    int downVoteCost = 0;
    
    public void IncreaseVote()
    {
        numVotes++;

        voteNumber.text = numVotes.ToString();

        //Send empty to server
    }

    public void DecreaseVote()
    {
        numVotes--;
        voteNumber.text = numVotes.ToString();

        //Send empty to server
    }

    public void LockInVote()
    {
        //Send empty, all info is stored on the server.
        voteUI.SetActive(false);
    }
}
