using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NeverTarget : RoleAbility
{
    [SerializeField] HoLPlayerSet waspPlayers;
    [SerializeField] HoLPlayerSet beePlayers;

    protected override void OnRoleGiven()
    {
        waspPlayers.Value.Shuffle();
        foreach (HoLPlayer ply in waspPlayers.Value)
        {
            ply.Target.OnVariableChanged += OnTargetChosen;

            if (ply.Target == Owner)
            {
                HoLPlayer newTarget = Owner;
                ReRandomiseTarget(ref newTarget);
                ply.Target.Value = newTarget;
                return;
            }
        }
    }

    [Server]
    void OnTargetChosen(HoLPlayer oldTarget, ref HoLPlayer newTarget)
    {
        ReRandomiseTarget(ref newTarget);
    }

    void ReRandomiseTarget(ref HoLPlayer target)
    {
        if (target != Owner) return;

        beePlayers.Value.Shuffle();

        foreach (HoLPlayer ply in beePlayers.Value)
        {
            if (ply == Owner) continue;
            target = ply;
            return;
        }
    }
}
