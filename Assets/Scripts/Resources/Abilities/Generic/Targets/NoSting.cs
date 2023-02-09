using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSting : RoleAbility
{
    protected override void OnRoleGiven()
    {
        if (Owner.Target.Value != null)
        {
            Owner.Target.Value = null;
            return;
        }
        Owner.Target.OnVariableChanged += RemoveTarget;
    }

    void RemoveTarget(HoLPlayer _, ref HoLPlayer target)
    {
        target = null;
    }
}
