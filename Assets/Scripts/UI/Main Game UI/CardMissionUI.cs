using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class CardMissionUI : NetworkBehaviour
{
    #region CLIENT

    [SerializeField] GameObject UI;
    [SerializeField] Image drawResult;
    [SerializeField] TMP_Text drawCost;
    [SerializeField] GameObject drawButton;
    [SerializeField] GameObject submitButton;

    [Tooltip("Returns true if the player is on the mission")]
    [SerializeField] BoolVariable isOnMission;

    //Whether the client has closed the vote result popup yet
    bool closedVotePopup;
    //Whether the mission has started for the client yet
    bool missionStartedYet;
    #endregion
    #region SERVER

    [Tooltip("The current round of the game")]
    [SerializeField] IntVariable roundNum;

    [Tooltip("What draws are required to succeed the mission")]
    [SerializeField] IntVariable missionDifficulty;

    [Tooltip("Invoked when a player draws a card")]
    [SerializeField] NetworkingEvent playerDrew;

    [Tooltip("Invoked when a player plays a card")]
    [SerializeField] NetworkingEvent playerPlayed;

    [Tooltip("All players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    #endregion

    [Server]
    public void AfterDeckCreated()
    {
        foreach (HoLPlayer ply in allPlayers.Value) 
        {
            ply.Deck.Value.OnDraw += card => ReceiveDrawResultFromServer(ply.connectionToClient, card.Sprite);
            ply.NextDrawCost.AfterVariableChanged += val => OnDrawCostChanged(ply.connectionToClient, val);
        };
    }

    /// <summary>
    /// Called when the cards mission starts
    /// </summary>
    [ClientRpc]
    public void ClientMissionStarted()
    {
        if (!isOnMission) return;
        missionStartedYet = true;

        if (!closedVotePopup) return;
        UI.SetActive(true);
        PlayerDrewCard();
        missionStartedYet = false;
        closedVotePopup = false;
    }

    [Client]
    public void OnClosedVotePopup()
    {
        closedVotePopup = true;

        if (!missionStartedYet) return;
        UI.SetActive(true);
        PlayerDrewCard();
        missionStartedYet = false;
        closedVotePopup = false;
    }

    [TargetRpc]
    public void TargetActivateMissionUI(NetworkConnection conn, Sprite sprite, int cost)
    {
        UI.SetActive(true);
        drawResult.sprite = sprite;
        drawCost.text = cost.ToString();
    }

    /// <summary>
    /// Called when the draw button is clicked on this client
    /// </summary>
    [Client]
    public void DrawNewCard()
    {
        PlayerDrewCard();
    }

    [Command(requiresAuthority = false)]
    public void PlayerDrewCard(NetworkConnectionToClient conn = null)
    {
        playerDrew?.Invoke(conn);
    }

    /// <summary>
    /// The server tells us what we drew
    /// </summary>
    [TargetRpc]
    void ReceiveDrawResultFromServer(NetworkConnection conn, Sprite sprite)
    {
        drawResult.sprite = sprite;
    }

    [TargetRpc]
    void OnDrawCostChanged(NetworkConnection conn, int val)
    {
        drawCost.text = val.ToString();
    }

    /// <summary>
    /// Called when the submit button is clicked on this client
    /// </summary>
    [Client]
    public void PlayCard()
    {
        UI.SetActive(false);
        PlayerPlayedCard();
    }

    [ClientRpc]
    public void CloseUI()
    {
        UI.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    void PlayerPlayedCard(NetworkConnectionToClient conn = null)
    {
        playerPlayed?.Invoke(conn);
    }
}
