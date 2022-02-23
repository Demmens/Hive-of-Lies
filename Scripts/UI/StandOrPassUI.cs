using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class StandOrPassUI : MonoBehaviour
{
    [SerializeField] GameObject UI;
    [SerializeField] FavourController Favour;
    [SerializeField] TMPro.TMP_Text FavourCost;
    [SerializeField] GameObject StandButton;

    int favourCost;
    bool stood;

    private void Start()
    {
        //Listen for server sending information over
        NetworkClient.RegisterHandler<StartStandOrPassMsg>(CreateUI);
        ClientEventProvider.singleton.OnTeamLeaderChanged += TeamLeaderDecided;
    }

    public void CreateUI(StartStandOrPassMsg msg)
    {
        favourCost = msg.favourCost;
        FavourCost.text = $"{favourCost}f";
        //If we can't afford to stand, disable the button.
        if (favourCost > Favour.Favour) StandButton.SetActive(false);
        UI.SetActive(true);
    }

    public void ClickButton(bool standing)
    {
        UI.SetActive(false);

        Favour.Favour -= favourCost;

        NetworkClient.Send(new PlayerStandOrPassMsg()
        {
            isStanding = standing
        });
    }

    /// <summary>
    /// If the team leader is decided and it wasn't you, refund the cost for standing.
    /// </summary>
    /// <param name="msg"></param>
    public void TeamLeaderDecided(TeamLeaderChangedMsg msg)
    {
        if (stood && msg.ID != SteamUser.GetSteamID())
        {
            Favour.Favour += favourCost;
        }
    }
}
