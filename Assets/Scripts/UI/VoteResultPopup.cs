using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class VoteResultPopup : MonoBehaviour
{
    [SerializeField] GameObject popup;
    [SerializeField] GameObject continueButton;
    [SerializeField] List<PlayerVoteGameObject> allPlayerVotes;
    [SerializeField] TMP_Text voteTotal;

    [SerializeField] Color unreadyColour;
    [SerializeField] Color readyColour;

    /// <summary>
    /// The player vote, and the associated gameobject
    /// </summary>
    Dictionary<CSteamID, PlayerVoteGameObject> playerVotes;
    void Start()
    {
        NetworkClient.RegisterHandler<SendVoteResultMsg>(ReceiveVoteResults);
        NetworkClient.RegisterHandler<VoteContinueClickedMsg>(OnPlayerClosedPopup);
    }

    void ReceiveVoteResults(SendVoteResultMsg msg)
    {
        playerVotes = new Dictionary<CSteamID, PlayerVoteGameObject>();
        msg.votes.Sort((a, b) => { return a.votes - b.votes; });

        int total = 0;

        for (int i = 0; i < msg.votes.Count; i++)
        {
            playerVotes.Add(msg.votes[i].ply, allPlayerVotes[i]);
            allPlayerVotes[i].continued.color = unreadyColour;
            allPlayerVotes[i].name.text = SteamFriends.GetFriendPersonaName(msg.votes[i].ply);
            allPlayerVotes[i].vote.text = msg.votes[i].votes.ToString();
            total += msg.votes[i].votes;
            allPlayerVotes[i].obj.SetActive(true);
        }

        voteTotal.text = total.ToString();

        popup.SetActive(true);
    }

    /// <summary>
    /// Called when the continue button is clicked on this client
    /// </summary>
    public void ContinueClicked()
    {
        continueButton.SetActive(false);
        NetworkClient.Send(new VoteContinueClickedMsg() { });
    }

    /// <summary>
    /// Called when any client closes the popup
    /// </summary>
    void OnPlayerClosedPopup(VoteContinueClickedMsg msg)
    {
        if (!msg.lastPlayer)
        {
            playerVotes.TryGetValue(msg.closedBy, out PlayerVoteGameObject obj);
            obj.continued.color = readyColour;
        }
        else
        {
            popup.SetActive(false);
        }
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