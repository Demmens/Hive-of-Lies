using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KnowAllWasps : RoleAbility
{
    [SerializeField] HivePlayerSet waspPlayers;

    [SerializeField] GameObject popup;

    public void AfterAllRolesGiven()
    {
        string wasps = "";

        foreach (HivePlayer ply in waspPlayers.Value)
        {
            if (ply == Owner) continue;
            wasps += ply.DisplayName + "\n";
        }
        wasps = wasps.TrimEnd('\n');
        MakePopup(wasps);
    }

    [TargetRpc]
    void MakePopup(string wasps)
    {
        popup = Instantiate(popup);
        popup.GetComponent<Notification>().SetText("The wasps are:\n" + wasps);
    }
}
