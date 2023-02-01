using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AlwaysATarget : RoleAbility
{
    [SerializeField] HoLPlayerSet allPlayers;

    bool isTarget = false;

    public override void OnStartServer()
    {
        foreach (HoLPlayer ply in allPlayers.Value)
        {
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
