using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAddToMission : PlayerButtonDropdownItem
{
    public void AddToMission(bool added)
    {
        Debug.Log($"Player = {playerClicked}");
        ClientSelectPartners.singleton.PlayerPicked(playerClicked, added);
    }
}
