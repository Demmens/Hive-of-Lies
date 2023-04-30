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
    void SendUI(int standCost)
    {
        CreateUI(standCost);
    }

    [Client]
    public void CreateUI(int standCost)
    {
        if (!alive) return;
        FavourCost.text = $"{standCost}f";
        //Only allow clicking on the button if we can afford to stand
        StandButton.GetComponent<Button>().interactable = standCost <= favour;
        UI.SetActive(true);
    }

    [Client]
    public void ClickButton(bool standing)
    {
        UI.SetActive(false);
        PlayerStood(standing);
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
}