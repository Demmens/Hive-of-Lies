using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class VoteResultPopup : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] GameObject popup;
    [SerializeField] GameObject playerVotePrefab;
    [SerializeField] Transform clientVoteTotal;

    List<GameObject> playerVotes = new();

    int upvotesSoFar = 0;

    [SerializeField] GameEvent clientClosedPopup;
    #endregion
    #region SERVER

    [Tooltip("All the votes so far")]
    [SerializeField] VoteSet allVotes;

    [Tooltip("The total of all votes")]
    [SerializeField] IntVariable voteTotal;
    #endregion

    [Server]
    public override void OnStartServer()
    {
        //Only let the clients know if the vote is an upvote or downvote. They can't know how many votes have been placed.
        allVotes.AfterItemAdded += vote => VoteReceived(vote.ply.DisplayName, vote.votes >= 0 ? 1 : -1);
    }

    [ClientRpc]
    void VoteReceived(string name, int vote)
    {
        PlayerVoteObject voteObj = Instantiate(playerVotePrefab).GetComponent<PlayerVoteObject>();
        voteObj.transform.SetParent(popup.transform);
        //Place upvotes at the top and downvotes at the bottom, but randomise positions within those groups so we don't have weird metas around who voted first.
        int siblingIndex = vote > 0 ? Random.Range(1, 2 + upvotesSoFar) : Random.Range(1 + upvotesSoFar, popup.transform.childCount - 1);
        voteObj.transform.SetSiblingIndex(siblingIndex);
        voteObj.Name.text = name;
        voteObj.Vote.localScale = new Vector3(1, vote, 1);
        playerVotes.Add(voteObj.gameObject);

        if (vote > 0) upvotesSoFar++;
    }

    [Server]
    public void ReceiveVoteResults()
    {
        ClientReceiveVoteResults(voteTotal);
    }

    [ClientRpc]
    public void ClientReceiveVoteResults(int total)
    {
        float y = total >= 0 ? 1 : -1;
        clientVoteTotal.localScale = new Vector3(1, y, 1);

        popup.SetActive(true);
    }

    /// <summary>
    /// Called when the close button is clicked on this client
    /// </summary>
    [Client]
    public void ClosePopup()
    {
        popup.SetActive(false);

        foreach (GameObject obj in playerVotes) Destroy(obj);
        playerVotes = new();
        upvotesSoFar = 0;
        clientClosedPopup?.Invoke();
    }
}