using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class BlackWidowAbility : RoleAbility
{
    [SerializeField] int maxCost = 2;
    [SerializeField] HivePlayerSet playersOnMission;
    bool isSolo = false;

    protected override void OnRoleGiven()
    {
        Owner.NextDrawCost.OnVariableChanged += ModifyDrawCost;
    }

    public void MissionStarted()
    {
        isSolo = true;
        foreach (HivePlayer ply in playersOnMission.Value)
        {
            if (ply == Owner) continue;
            if (ply.Team.Value.Team == Team.Wasp) isSolo = false;
        }
    }

    void ModifyDrawCost(int oldCost, ref int newCost)
    {
        if (isSolo) newCost = Mathf.Min(newCost, maxCost);
    }
}
