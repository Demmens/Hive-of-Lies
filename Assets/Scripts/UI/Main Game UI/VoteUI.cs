using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class VoteUI : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] TMP_Text voteNumber;
    [SerializeField] TMP_Text yesCost;
    [SerializeField] TMP_Text noCost;
    [SerializeField] Transform submitThumb;

    [SerializeField] Button yesVote;
    [SerializeField] Button noVote;
    [SerializeField] Button submitButton;

    [SerializeField] GameObject voteUI;

    [Tooltip("The amount of favour the local player has")]
    [SerializeField] IntVariable favour;

    [Tooltip("How many votes the local player has placed")]
    [SerializeField] IntVariable numVotes;

    [Tooltip("Whether this player is alive")]
    [SerializeField] BoolVariable alive;
    #endregion

    #region SERVER
    [Tooltip("Invoked when a player increases their vote")]
    [SerializeField] NetworkingEvent increasedVote;

    [Tooltip("Invoked when a player decreases their vote")]
    [SerializeField] NetworkingEvent decreasedVote;

    [Tooltip("Invoked when a player locks in their vote")]
    [SerializeField] NetworkingEvent lockedIn;

    [Tooltip("All players in the game")]
    [SerializeField] hivePlayerSet allPlayers;
    #endregion

    [Server]
    public void AfterSetup()
    {
        foreach (hivePlayer ply in allPlayers.Value)
        {
            ply.NextUpvoteCost.AfterVariableChanged += (val) => ReceiveUpvoteCost(ply.connectionToClient, val);
            ply.NextDownvoteCost.AfterVariableChanged += (val) => ReceiveDownvoteCost(ply.connectionToClient, val);
            ply.NumVotes.AfterVariableChanged += (val) => ReceiveNumVotes(ply.connectionToClient, val);
        }
    }

    /// <summary>
    /// Called when the vote starts
    /// </summary>
    /// <param name="msg"></param>
    [ClientRpc]
    public void VoteStarted()
    {
        if (!alive) return;
        voteUI.SetActive(true);
        numVotes.Value = 0;
    }

    [TargetRpc]
    public void TargetEnableUI(NetworkConnection conn)
    {
        voteUI.SetActive(true);
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
        yesCost.text = cost < 0 ? $"+{-cost}" : cost.ToString();

        yesVote.interactable = cost <= favour || cost <= 0;
    }

    /// <summary>
    /// Called when a player deceases their vote (downvotes)
    /// </summary>
    [Client]
    public void DecreaseVote()
    {
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
        noCost.text = cost < 0 ? $"+{-cost}" : cost.ToString();

        noVote.interactable = cost <= favour || cost <= 0;
    }

    [TargetRpc]
    void ReceiveNumVotes(NetworkConnection conn, int num)
    {
        voteNumber.text = Mathf.Abs(num).ToString();

        float y = num >= 0 ? 1f : -1f;
        submitThumb.localScale = new Vector3(1, y, 1); 
        submitButton.interactable = num != 0;
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
