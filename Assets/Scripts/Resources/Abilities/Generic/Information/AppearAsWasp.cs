using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearAsWasp : RoleAbility
{
    [SerializeField] hivePlayerSet supposedWasps;
    protected override void OnRoleGiven()
    {
        supposedWasps.Add(Owner);
        Owner.OnGetTeam += (ref Team team) => team = Team.Wasp;
    }
}
