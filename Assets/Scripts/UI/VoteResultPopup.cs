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
    [SerializeField] GameObject continueButton;
    [SerializeField] List<PlayerVoteGameObject> allPlayerVotes;
    [SerializeField] TMP_Text voteTotal;

    [SerializeField] Color unreadyColour;
    [SerializeField] Color readyColour;

    /// <summary>
    /// The player vote, and the associated gameobject
    /// </summary>
    Dictionary<ulong, PlayerVoteGameObject> playerVotes;
    List<ClientPlayerVote> votesReceived = new();
    #endregion
    #region SERVER

    [Tooltip("All the votes so far")]
    [SerializeField] VoteSet allVotes;

    [Tooltip("All the votes so far")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("All players by their network connection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("Invoked when all players have closed this popup")]
    [SerializeField] GameEvent allPlayersClosedPopup;

    /// <summary>
    /// A list of all players that have closed this popup so far.
    /// </summary>
    List<HoLPlayer> playersClosedPopup = new();

    #endregion

    [Server]
    public override void OnStartServer()
    {
        allVotes.AfterItemAdded += vote => VoteReceived(vote.ply.PlayerID, vote.ply.DisplayName, vote.votes);
    }

    [ClientRpc]
    void VoteReceived(ulong player, string name, int vote)
    {
        votesReceived.Add(new ClientPlayerVote
        {
            id = player,
            name = name,
            votes = vote,
        });
    }

    [ClientRpc]
    public void ReceiveVoteResults()
    {
        continueButton.SetActive(true);
        playerVotes = new();
        votesReceived.Sort((a, b) => { return a.votes - b.votes; });

        int total = 0;

        for (int i = 0; i < votesReceived.Count; i++)
        {
            ClientPlayerVote vote = votesReceived[i];
            PlayerVoteGameObject voteObj = allPlayerVotes[i];
            playerVotes.Add(vote.id, voteObj);
            voteObj.continued.color = unreadyColour;
            voteObj.name.text = vote.name;
            voteObj.vote.text = vote.votes.ToString();
            total += vote.votes;
            voteObj.obj.SetActive(true);
        }

        voteTotal.text = total.ToString();

        popup.SetActive(true);
    }

    /// <summary>
    /// Called when the continue button is clicked on this client
    /// </summary>
    [Client]
    public void ContinueClicked()
    {
        continueButton.SetActive(false);
        ServerOnClosedPopup();
    }

    [Command(requiresAuthority = false)]
    void ServerOnClosedPopup(NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        if (playersClosedPopup.Contains(ply)) return;

        playersClosedPopup.Add(ply);

        bool isLastPlayer = playersClosedPopup.Count >= playerCount;

        OnPlayerClosedPopup(ply.PlayerID, isLastPlayer);

        if (isLastPlayer)
        {
            allPlayersClosedPopup?.Invoke();
            playersClosedPopup = new();
        }
    }

    /// <summary>
    /// Called when any client closes the popup
    /// </summary>
    [ClientRpc]
    void OnPlayerClosedPopup(ulong closedBy, bool lastPlayer)
    {
        if (!lastPlayer)
        {
            playerVotes.TryGetValue(closedBy, out PlayerVoteGameObject obj);
            obj.continued.color = readyColour;
        }
        else
        {
            popup.SetActive(false);
            votesReceived = new();
        }
    }

    struct ClientPlayerVote
    {
        public ulong id;
        public string name;
        public int votes;
    }
}

[System.Serializable]
public struct PlayerVoteGameObject
{
    public GameObject obj;
    public Image continued;
    public TMP_Text name;
    public TMP_Text vote;
}