using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class BlackWidowAbility : RoleAbility
{
    [SerializeField] int maxCost = 2;
    [SerializeField] HoLPlayerSet playersOnMission;
    bool isSolo = false;

    void Start()
    {
        Owner.NextDrawCost.OnVariableChanged += ModifyDrawCost;
    }

    public void MissionStarted()
    {
        isSolo = true;
        foreach (HoLPlayer ply in playersOnMission.Value)
        {
            if (ply == Owner) continue;
            if (ply.Team == Team.Wasp) isSolo = false;
        }
    }

    void ModifyDrawCost(int oldCost, ref int newCost)
    {
        if (isSolo) newCost = Mathf.Min(newCost, maxCost);
    }
}
