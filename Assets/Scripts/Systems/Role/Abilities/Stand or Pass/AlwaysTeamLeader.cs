using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysTeamLeader : RoleAbility
{
    [SerializeField] HivePlayerVariable teamLeader;
    [SerializeField] HivePlayerSet standingPlayers;
    void Start()
    {
        teamLeader.OnVariableChanged += OnTeamLeaderChange;
    }

    void OnTeamLeaderChange(HivePlayer oldVal, ref HivePlayer newVal)
    {
        if (!standingPlayers.Value.Contains(Owner)) return;
        //Only activate if there isn't currently a Team Leader (during the stand or pass)
        if (oldVal != null) return;

        newVal = Owner;
    }
}