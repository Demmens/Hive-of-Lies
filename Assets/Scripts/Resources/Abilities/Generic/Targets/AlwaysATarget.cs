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
            //If they alreay have a target, we should assign ourselves to be the target instead
            if (ply.Target != null && !isTarget)
            {
                ply.Target.Value = Owner;
                isTarget = true;
                //Still listen for the target changing in case we get unset as a target
                ply.Target.OnVariableChanged += OnTargetChosen;
                return;
            }
            //If they don't have a target, listen for when they get one
            ply.Target.OnVariableChanged += OnTargetChosen;
        }
    }

    [Server]
    void OnTargetChosen(HoLPlayer oldTarget, ref HoLPlayer newTarget)
    {
        //If we're being unset as a target for whatever reason
        if (oldTarget == Owner && newTarget != Owner)
        {
            OnOwnerUnset();
            return;
        }

        if (isTarget) return;

        newTarget = Owner;
        isTarget = true;
    }

    void OnOwnerUnset()
    {
        isTarget = false;
        foreach (HoLPlayer ply in allPlayers.Value)
        {
            //Find any wasp who already has a target, and replace us as a target for them.
            if (ply.Target != null)
            {
                ply.Target.Value = Owner;
                isTarget = true;
            }
        }
    }
}
