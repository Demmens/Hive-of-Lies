using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Steamworks;

public class StandOrPassUI : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] GameObject UI;
    [SerializeField] TMPro.TMP_Text FavourCost;
    [SerializeField] GameObject StandButton;

    [Tooltip("How much favour the local player has")]
    [SerializeField] IntVariable favour;
    [Tooltip("Whether the local player is alive or not")]
    [SerializeField] BoolVariable alive;

    bool closedMissionPopup = true;
    bool phaseBegun;
    int standCost;
    #endregion
    #region SERVER
    [Tooltip("The currently active mission")]
    [SerializeField] MissionVariable currentMission;
    [Tooltip("Invoked when a client stands for team leader")]
    [SerializeField] NetworkingEvent onPlayerStood;
    [Tooltip("Invoked when a client passes on standing for team leader")]
    [SerializeField] NetworkingEvent onPlayerPassed;
    #endregion

    [Server]
    public void StandOrPassBegin()
    {
        SendUI(currentMission.Value.FavourCost);
    }

    [ClientRpc]
    void SendUI(int cost)
    {
        standCost = cost;
        phaseBegun = true;
        if (!closedMissionPopup) return;
        CreateUI(cost);
    }

    [Client]
    public void MissionPopupClosed()
    {
        closedMissionPopup = true;
        if (!phaseBegun) return;

        CreateUI(standCost);
    }

    [Client]
    public void CreateUI(int standCost)
    {
        if (!alive) return;
        FavourCost.text = standCost.ToString();
        //Only allow clicking on the button if we can afford to stand
        StandButton.GetComponent<Button>().interactable = standCost <= favour;
        UI.SetActive(true);
    }

    [Client]
    public void ClickButton(bool standing)
    {
        UI.SetActive(false);
        PlayerStood(standing);
        phaseBegun = false;
    }

    [Command(requiresAuthority = false)]
    void PlayerStood(bool standing, NetworkConnectionToClient conn = null)
    {
        if (standing)
        {
            onPlayerStood?.Invoke(conn);
        }
        else
        {
            onPlayerPassed?.Invoke(conn);
        }
    }

    public void OnMissionStart()
    {
        closedMissionPopup = false;
    }
}