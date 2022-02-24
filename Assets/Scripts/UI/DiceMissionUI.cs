using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;

public class DiceMissionUI : MonoBehaviour
{
    [SerializeField] GameObject UI;
    [SerializeField] TMP_Text RollResult;
    [SerializeField] TMP_Text RollCost;
    [SerializeField] TMP_Text RollNeeded;
    [SerializeField] GameObject RollButton;
    [SerializeField] GameObject SubmitButton;

    [SerializeField] CostCalculation costCalc;
    [SerializeField] FavourController favourController;

    /// <summary>
    /// The number of rolls we've used so far
    /// </summary>
    int numRolls;

    int _nextRerollCost;
    /// <summary>
    /// The cost of the next reroll
    /// </summary>
    int nextRerollCost
    {
        get
        {
            return _nextRerollCost;
        }
        set
        {
            _nextRerollCost = value;
            RollCost.text = $"{value}f";
        }
    }

    private void Start()
    {
        NetworkClient.RegisterHandler<DiceMissionStartedMsg>(MissionStarted);
        ClientEventProvider.singleton.OnPlayerRolled += ReceiveRollResultFromServer;
        NetworkClient.RegisterHandler<PlayerLockedRollMsg>(PlayerSubmitted);
    }

    /// <summary>
    /// Called when the dice mission starts
    /// </summary>
    void MissionStarted(DiceMissionStartedMsg msg)
    {
        CSteamID steamID = SteamUser.GetSteamID();
        if (ClientGameInfo.CurrentlySelected.Contains(steamID) || ClientGameInfo.TeamLeaderID == steamID)
        {
            RollResult.text = "0";
            numRolls = 0;
            nextRerollCost = 0;
            SubmitButton.SetActive(false);
            RollButton.SetActive(true);
            UI.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the roll button is clicked on this client
    /// </summary>
    public void RerollDice()
    {
        favourController.Favour -= nextRerollCost;
        RollResult.text = "-";
        numRolls++;

        nextRerollCost = costCalc.CalculateRerollCost(SteamUser.GetSteamID(), numRolls);
        if (favourController.Favour < nextRerollCost) RollButton.SetActive(false);

        NetworkClient.Send(new PlayerRolledMsg() { });
    }

    /// <summary>
    /// The server tells us what we rolled
    /// </summary>
    void ReceiveRollResultFromServer(PlayerRolledMsg msg)
    {
        RollResult.text = msg.rollResult.ToString();
    }

    /// <summary>
    /// Called when the submit button is clicked on this client
    /// </summary>
    public void LockInDice()
    {
        SubmitButton.SetActive(false);
        NetworkClient.Send(new PlayerLockedRollMsg() { });
    }

    /// <summary>
    /// Called when any client locks in their roll
    /// </summary>
    void PlayerSubmitted(PlayerLockedRollMsg msg)
    {
        if (msg.lastPlayer)
        {
            UI.SetActive(false);
        }
    }
}
