using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;

public class VoteUI : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] TMP_Text voteNumber;
    [SerializeField] TMP_Text yesCost;
    [SerializeField] TMP_Text noCost;

    [SerializeField] GameObject yesVote;
    [SerializeField] GameObject noVote;
 
    [SerializeField] GameObject voteUI;
    [SerializeField] CostCalculation costCalc;

    [Tooltip("The amount of favour the local player has")]
    [SerializeField] IntVariable favour;

    [Tooltip("How many votes the local player has placed")]
    [SerializeField] IntVariable numVotes;

    int upVoteCost = 0;
    int downVoteCost = 0;
    #endregion
    #region SERVER
    [Tooltip("Invoked when a player increases their vote")]
    [SerializeField] NetworkingEvent increasedVote;

    [Tooltip("Invoked when a player decreases their vote")]
    [SerializeField] NetworkingEvent decreasedVote;
    #endregion

    public override void OnStartClient()
    {
        numVotes.AfterVariableChanged += VoteNumberChanged;
    }

    /// <summary>
    /// Called when the vote starts
    /// </summary>
    /// <param name="msg"></param>
    public void VoteStarted()
    {
        voteUI.SetActive(true);
        numVotes.Value = 0;
    }

    /// <summary>
    /// Called when a player increases their vote (upvotes)
    /// </summary>
    [Client]
    public void IncreaseVote()
    {
        numVotes++;

        noVote.SetActive(true);
        if (upVoteCost > favour) yesVote.SetActive(false);

        noCost.text = yesCost.text;
        yesCost.text = "~f";

        PlayerIncreasedVote();
    }

    [Command(requiresAuthority = false)]
    void PlayerIncreasedVote(NetworkConnectionToClient conn = null)
    {
        increasedVote?.Invoke(conn);
    }

    /// <summary>
    /// Called when a player deceases their vote (downvotes)
    /// </summary>
    public void DecreaseVote()
    {
        numVotes--;

        yesCost.text = noCost.text;
        noCost.text = "~f";
        yesVote.SetActive(true);
        if (downVoteCost > favour) noVote.SetActive(false);
        PlayerDecreasedVote();
    }

    [Command(requiresAuthority = false)]
    void PlayerDecreasedVote(NetworkConnectionToClient conn = null)
    {
        decreasedVote?.Invoke(conn);
    }

    /// <summary>
    /// Recalculates the cost of increasing or decreasing your vote
    /// </summary>
    [Client]
    void VoteNumberChanged(int newValue)
    {
        if (numVotes > 0)
        {

            downVoteCost *= -1;

            noCost.text = $"{-downVoteCost}f";
            yesCost.text = $"{-upVoteCost}f";
        }
        else
        {

            noCost.text = $"{-downVoteCost}f";
            yesCost.text = $"{-upVoteCost}f";

            upVoteCost *= -1;
        }
    }

    [Client]
    public void LockInVote()
    {
        voteUI.SetActive(false);
        OnPlayerLockedIn();
    }

    [Command(requiresAuthority = false)]
    void OnPlayerLockedIn(NetworkConnectionToClient conn = null)
    {

    }
}
