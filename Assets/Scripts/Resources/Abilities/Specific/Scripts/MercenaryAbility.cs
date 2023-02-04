using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MercenaryAbility : RoleAbility
{
    [SerializeField] int favourGain = 5;
    [SerializeField] HoLPlayerSet playersOnMission;
    [SerializeField] HoLPlayerVariable teamLeader;
    
    public void ChoicesLockedIn()
    {
        if (Owner == teamLeader.Value) return;
        if (!playersOnMission.Value.Contains(Owner)) return;
        Owner.Favour.Value += favourGain;
    }
}
