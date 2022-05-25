using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CardMissionUI : MonoBehaviour
{
    [SerializeField] GameObject UI;
    [SerializeField] TMP_Text drawResult;
    [SerializeField] TMP_Text drawButtonText;
    [SerializeField] TMP_Text needed;
    [SerializeField] GameObject drawButton;
    [SerializeField] GameObject submitButton;

    [SerializeField] CostCalculation costCalc;
    [SerializeField] FavourController favourController;

    /// <summary>
    /// The number of rolls we've used so far
    /// </summary>
    int numDraws;

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
            drawButtonText.text = $"Reroll ({value}f)";
        }
    }

    private void Start()
    {
        NetworkClient.RegisterHandler<CardMissionStartedMsg>(MissionStarted);
        ClientEventProvider.singleton.OnPlayerDrew += ReceiveDrawResultFromServer;
        NetworkClient.RegisterHandler<PlayerPlayedMsg>(PlayerSubmitted);
    }

    /// <summary>
    /// Called when the cards mission starts
    /// </summary>
    void MissionStarted(CardMissionStartedMsg msg)
    {
        Debug.Log("Mission started on client");
        if (ClientGameInfo.CurrentlySelected.Contains(ClientGameInfo.PlayerID))
        {
            Debug.Log("Mission started. Player is on mission.");
            drawResult.text = "0";
            numDraws = 0;
            nextDrawCost = 0;
            submitButton.SetActive(false);
            drawButton.SetActive(true);
            UI.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the draw button is clicked on this client
    /// </summary>
    public void DrawNewCard()
    {
        favourController.Favour -= nextDrawCost;
        drawResult.text = "-";
        numDraws++;
        drawButton.SetActive(false);
        submitButton.SetActive(false);

        NetworkClient.Send(new DrawCardMsg() { });
    }

    /// <summary>
    /// The server tells us what we drew
    /// </summary>
    void ReceiveDrawResultFromServer(DrawCardMsg msg)
    {
        Debug.Log("Received draw result from the server");
        drawResult.text = msg.drawnCard.Value.ToString();
        submitButton.SetActive(true);
        drawButton.SetActive(true);

        nextDrawCost = costCalc.CalculateDrawCost(ClientGameInfo.PlayerID, numDraws);
        if (favourController.Favour < nextDrawCost) drawButton.SetActive(false);
    }

    /// <summary>
    /// Called when the submit button is clicked on this client
    /// </summary>
    public void PlayCard()
    {
        submitButton.SetActive(false);
        drawButton.SetActive(false);
        NetworkClient.Send(new PlayerPlayedMsg() { });
    }

    /// <summary>
    /// Called from the server when any client locks in their draw
    /// </summary>
    private void PlayerSubmitted(PlayerPlayedMsg msg)
    {
        if (msg.lastPlayer)
        {
            UI.SetActive(false);
        }
    }
}
