using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysTeamLeader : RoleAbility
{
    [SerializeField] hivePlayerVariable teamLeader;
    [SerializeField] hivePlayerSet standingPlayers;
    void Start()
    {
        teamLeader.OnVariableChanged += OnTeamLeaderChange;
    }

    void OnTeamLeaderChange(hivePlayer oldVal, ref hivePlayer newVal)
    {
        if (!standingPlayers.Value.Contains(Owner)) return;
        //Only activate if there isn't currently a Team Leader (during the stand or pass)
        if (oldVal != null) return;

        newVal = Owner;
    }
}