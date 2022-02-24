using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class VoteResultPopup : MonoBehaviour
{
    [SerializeField] GameObject popup;
    [SerializeField] GameObject continueButton;

    [SerializeField] TMP_Text waitingFor;
    void Start()
    {
        NetworkClient.RegisterHandler<SendVoteResultMsg>(ReceiveVoteResults);
        NetworkClient.RegisterHandler<VotePopupClosedMsg>(OnPlayerClosedPopup);
    }

    void ReceiveVoteResults(SendVoteResultMsg msg)
    {
        msg.votes.Sort((a, b) => {return a.votes - b.votes;});

        //Display vote information. Can't really do this until I'm at my pc.

        msg.votes.ForEach(vote =>
        {
            waitingFor.text += $"{SteamFriends.GetFriendPersonaName(vote.ply)}\n";
        });

        popup.SetActive(true);
    }

    /// <summary>
    /// Called when the continue button is clicked on this client
    /// </summary>
    public void ContinueClicked()
    {
        continueButton.SetActive(false);
        NetworkClient.Send(new VotePopupClosedMsg() { });
    }

    /// <summary>
    /// Called when any client closes the popup
    /// </summary>
    void OnPlayerClosedPopup(VotePopupClosedMsg msg)
    {
        if (!msg.lastPlayer)
        {
            string playerName = SteamFriends.GetFriendPersonaName(msg.closedBy);
            //Remove the player from the "Waiting for" section
            waitingFor.text.Replace($"{playerName}\n", "");
        }
        else
        {
            popup.SetActive(false);
        }
    }
}
