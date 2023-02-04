using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AlwaysATarget : RoleAbility
{
    [SerializeField] HoLPlayerSet allPlayers;

    bool isTarget = false;

    protected override void OnRoleGiven()
    {
        allPlayers.Value.Shuffle();
        foreach (HoLPlayer ply in allPlayers.Value)
        {
            if (ply.Target != null && !isTarget)
            {
                ply.Target.Value = Owner;
                isTarget = true;
                return;
            }
            ply.Target.OnVariableChanged += OnTargetChosen;
        }
    }

    [Server]
    void OnTargetChosen(HoLPlayer oldTarget, ref HoLPlayer newTarget)
    {
        if (isTarget) return;

        newTarget = Owner;
        isTarget = true;
    }
}
