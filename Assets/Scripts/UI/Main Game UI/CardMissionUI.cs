using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CardMissionUI : NetworkBehaviour
{
    #region CLIENT

    [SerializeField] GameObject UI;
    [SerializeField] TMP_Text drawResult;
    [SerializeField] TMP_Text drawButtonText;
    [SerializeField] GameObject drawButton;
    [SerializeField] GameObject submitButton;

    [Tooltip("Returns true if the player is on the mission")]
    [SerializeField] BoolVariable isOnMission;

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
        allPlayers.Value.ForEach(ply => 
        {
            ply.Deck.Value.OnDraw += card => ReceiveDrawResultFromServer(ply.Connection, card.TempValue);
            ply.NextDrawCost.AfterVariableChanged += val => OnDrawCostChanged(ply.Connection, val);
        });
    }

    /// <summary>
    /// Called when the cards mission starts
    /// </summary>
    [ClientRpc]
    public void ClientMissionStarted()
    {
        if (!isOnMission) return;

        drawResult.text = "0";
        drawButtonText.text = $"Redraw (0f)";
        submitButton.SetActive(false);
        drawButton.SetActive(true);
        UI.SetActive(true);
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
    void PlayerDrewCard(NetworkConnectionToClient conn = null)
    {
        playerDrew?.Invoke(conn);
    }

    /// <summary>
    /// The server tells us what we drew
    /// </summary>
    [TargetRpc]
    void ReceiveDrawResultFromServer(NetworkConnection conn, int value)
    {
        drawResult.text = value.ToString();
        submitButton.SetActive(true);
        drawButton.SetActive(true);
    }

    [TargetRpc]
    void OnDrawCostChanged(NetworkConnection conn, int val)
    {
        drawButtonText.text = $"Redraw ({val}f)";
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
