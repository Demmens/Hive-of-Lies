using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearAsWasp : RoleAbility
{
    [SerializeField] HivePlayerSet supposedWasps;
    [SerializeField] ETeam waspTeam;
    protected override void OnRoleGiven()
    {
        supposedWasps.Add(Owner);
        Owner.OnGetTeam += (ref ETeam team) => team = waspTeam;
    }
}
