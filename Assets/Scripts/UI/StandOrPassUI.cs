using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class StandOrPassUI : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] GameObject UI;
    [SerializeField] TMPro.TMP_Text FavourCost;
    [SerializeField] GameObject StandButton;

    [Tooltip("How much favour the local player has")]
    [SerializeField] IntVariable favour;
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
        CreateUI(currentMission.Value.FavourCost);
    }

    [ClientRpc]
    void CreateUI(int standCost)
    {
        FavourCost.text = $"{standCost}f";
        //If we can't afford to stand, disable the button.
        if (standCost > favour) StandButton.SetActive(false);
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