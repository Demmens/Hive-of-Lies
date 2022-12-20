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
    #endregion

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

    public void ClickButton(bool standing)
    {
        UI.SetActive(false);
    }
}