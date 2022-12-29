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
    [SerializeField] TMP_Text needed;
    [SerializeField] GameObject drawButton;
    [SerializeField] GameObject submitButton;

    [Tooltip("Returns true if the player is on the mission")]
    [SerializeField] BoolVariable isOnMission;

    #endregion
    #region SERVER

    [Tooltip("Invoked when a player draws a card")]
    [SerializeField] NetworkingEvent playerDrew;

    [Tooltip("Invoked when a player plays a card")]
    [SerializeField] NetworkingEvent playerPlayed;

    [Tooltip("All players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    #endregion

    int _nextRerollCost;
    /// <summary>
    /// The cost of the next reroll
    /// </summary>
    int nextDrawCost
    {
        get
        {
            return _nextRerollCost;
        }
        set
        {
            _nextRerollCost = value;
            drawButtonText.text = $"Redraw ({value}f)";
        }
    }

    [Server]
    public void AfterRolesGiven()
    {
        allPlayers.Value.ForEach(ply => ply.Deck.Value.OnDraw += card => ReceiveDrawResultFromServer(ply.Connection, card.Value, ply.NextDrawCost));
    }

    /// <summary>
    /// Called when the cards mission starts
    /// </summary>
    [ClientRpc]
    public void MissionStarted()
    {
        Debug.Log("Mission started on client");

        if (!isOnMission) return;

        Debug.Log("Mission started. Player is on mission.");
        drawResult.text = "0";
        nextDrawCost = 0;
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
    void ReceiveDrawResultFromServer(NetworkConnection conn, int value, int nextCost)
    {
        drawResult.text = value.ToString();
        submitButton.SetActive(true);
        drawButton.SetActive(true);
        drawButtonText.text = $"Redraw ({nextCost}f)";
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

    [Command(requiresAuthority = false)]
    void PlayerPlayedCard(NetworkConnectionToClient conn = null)
    {
        playerPlayed?.Invoke(conn);
    }
}
