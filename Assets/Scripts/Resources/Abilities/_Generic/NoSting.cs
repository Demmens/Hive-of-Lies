using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSting : RoleAbility
{
    public override void OnRoleGiven()
    {
        Owner.Target.OnVariableChanged += RemoveTarget;
    }

    void RemoveTarget(HoLPlayer _, ref HoLPlayer target)
    {
        target = null;
    }
}
