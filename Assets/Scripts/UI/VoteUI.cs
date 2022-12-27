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

    //For the disabled button
    [SerializeField] TMP_Text yesCost2;
    [SerializeField] TMP_Text noCost2;

    [SerializeField] GameObject yesVote;
    [SerializeField] GameObject noVote;

    [SerializeField] GameObject voteUI;

    [Tooltip("The amount of favour the local player has")]
    [SerializeField] IntVariable favour;

    [Tooltip("How many votes the local player has placed")]
    [SerializeField] IntVariable numVotes;
    #endregion

    #region SERVER
    [Tooltip("Invoked when a player increases their vote")]
    [SerializeField] NetworkingEvent increasedVote;

    [Tooltip("Invoked when a player decreases their vote")]
    [SerializeField] NetworkingEvent decreasedVote;

    [Tooltip("Invoked when a player locks in their vote")]
    [SerializeField] NetworkingEvent lockedIn;

    [Tooltip("All players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;
    #endregion

    [Server]
    public void AfterSetup()
    {
        allPlayers.Value.ForEach(ply => ply.OnUpvoteCostChanged += ReceiveUpvoteCost);
        allPlayers.Value.ForEach(ply => ply.OnDownvoteCostChanged += ReceiveDownvoteCost);
        allPlayers.Value.ForEach(ply => ply.OnNumVotesChanged += ReceiveNumVotes);
    }

    /// <summary>
    /// Called when the vote starts
    /// </summary>
    /// <param name="msg"></param>
    [Client]
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
        PlayerIncreasedVote();
    }

    [Command(requiresAuthority = false)]
    void PlayerIncreasedVote(NetworkConnectionToClient conn = null)
    {
        increasedVote?.Invoke(conn);
    }

    [TargetRpc]
    void ReceiveUpvoteCost(NetworkConnection conn, int cost)
    {
        yesCost.text = $"{-cost}f";
        yesCost2.text = yesCost.text;
        if (cost > favour && cost > 0) yesVote.SetActive(false);
        else yesVote.SetActive(true);
    }

    /// <summary>
    /// Called when a player deceases their vote (downvotes)
    /// </summary>
    [Client]
    public void DecreaseVote()
    {
        yesVote.SetActive(true);

        PlayerDecreasedVote();
    }

    [Command(requiresAuthority = false)]
    void PlayerDecreasedVote(NetworkConnectionToClient conn = null)
    {
        decreasedVote?.Invoke(conn);
    }

    [TargetRpc]
    void ReceiveDownvoteCost(NetworkConnection conn, int cost)
    {
        noCost.text = $"{-cost}f";
        noCost2.text = noCost.text;
        if (cost > favour && cost > 0) noVote.SetActive(false);
        else noVote.SetActive(true);
    }

    [TargetRpc]
    void ReceiveNumVotes(NetworkConnection conn, int num)
    {
        voteNumber.text = num.ToString();
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
        lockedIn?.Invoke(conn);
    }
}
