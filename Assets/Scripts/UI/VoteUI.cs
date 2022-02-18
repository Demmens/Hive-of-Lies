using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteUI : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text voteNumber;

    [SerializeField] GameObject voteUI;

    int numVotes = 0;
    
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
