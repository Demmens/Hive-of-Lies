using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSting : RoleAbility
{
    public override void OnStartServer()
    {
        Owner.Target.OnVariableChanged += RemoveTarget;
    }

    void RemoveTarget(HoLPlayer _, ref HoLPlayer target)
    {
        target = null;
    }
}
