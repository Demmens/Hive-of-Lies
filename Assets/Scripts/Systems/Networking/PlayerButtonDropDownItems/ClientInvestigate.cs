using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInvestigate : PlayerButtonDropdownItem
{
    public void Investigate()
    {
        InvestigatePlayer.Singleton.PlayerInvestigated(playerClicked);
    }
}
