using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysTeamLeader : RoleAbility
{
    [SerializeField] HoLPlayerVariable teamLeader;
    [SerializeField] HoLPlayerSet standingPlayers;
    void Start()
    {
        teamLeader.OnVariableChanged += OnTeamLeaderChange;
    }

    void OnTeamLeaderChange(HoLPlayer oldVal, ref HoLPlayer newVal)
    {
        if (!standingPlayers.Value.Contains(Owner)) return;
        //Only activate if there isn't currently a Team Leader (during the stand or pass)
        if (oldVal != null) return;

        newVal = Owner;
    }
}