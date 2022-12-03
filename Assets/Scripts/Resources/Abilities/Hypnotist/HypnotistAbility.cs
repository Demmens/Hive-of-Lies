using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class HypnotistAbility : RoleAbility
{
    /*[SerializeField] int cost = 5;
    [SerializeField] GameObject button;
    [SerializeField] GameObject dropdownButton;
    DiceMission dice;
    RunMission miss;
    PlayerButtonDropdown dropdown;

    ClientEventProvider.PlayerID disabledEvents;
    void Start()
    {
        dice = DiceMission.singleton;
        miss = RunMission.singleton;
        dropdown = PlayerButtonDropdown.singleton;
        Instantiate(button);
        button.SetActive(false);

        miss.OnGamePhaseStart += DiceMissionStarted;
        miss.OnGamePhaseEnd += DiceMissionEnded;
    }

    void DiceMissionStarted()
    {
        PlayerButtonDropdownItem item = dropdown.AddItem(dropdownButton);
        SetButtonActive(Owner.connection, true);
    }

    void DiceMissionEnded()
    {
        dropdown.RemoveItem(dropdownButton);
        SetButtonActive(Owner.connection, false);
    }

    [TargetRpc]
    void SetButtonActive(NetworkConnection conn, bool active)
    {
        button.SetActive(active);
    }

    /// <summary>
    /// Called when a player is clicked on
    /// </summary>
    /// <param name="target"></param>
    [Command]
    public void ClickPlayer(CSteamID target)
    {
        if (!NetworkServer.active) return;
        if (Owner.Favour < cost) return;

        foreach (KeyValuePair<Player, PlayerRollInfo> pair in dice.RollInfo)
        {
            if (pair.Key.SteamID == target)
            {
                Owner.Favour -= cost;
                Owner.connection.Send(new ChangeFavourMsg() { favourIncrease = -5 });
                dice.RollDice(pair.Key);
            }
        }
    }*/
}