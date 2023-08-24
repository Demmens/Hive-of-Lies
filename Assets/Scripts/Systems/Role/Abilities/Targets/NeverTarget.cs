using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NeverTarget : RoleAbility
{
    [SerializeField] hivePlayerSet waspPlayers;
    [SerializeField] hivePlayerSet beePlayers;

    protected override void OnRoleGiven()
    {
        waspPlayers.Value.Shuffle();
        foreach (hivePlayer ply in waspPlayers.Value)
        {
            ply.Target.OnVariableChanged += OnTargetChosen;

            if (ply.Target == Owner)
            {
                hivePlayer newTarget = Owner;
                ReRandomiseTarget(ref newTarget);
                ply.Target.Value = newTarget;
                return;
            }
        }
    }

    [Server]
    void OnTargetChosen(hivePlayer oldTarget, ref hivePlayer newTarget)
    {
        ReRandomiseTarget(ref newTarget);
    }

    void ReRandomiseTarget(ref hivePlayer target)
    {
        if (target != Owner) return;

        beePlayers.Value.Shuffle();

        foreach (hivePlayer ply in beePlayers.Value)
        {
            if (ply == Owner) continue;
            target = ply;
            return;
        }
    }
}
