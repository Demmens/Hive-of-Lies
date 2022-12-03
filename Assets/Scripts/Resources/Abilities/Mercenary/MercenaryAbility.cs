using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MercenaryAbility : RoleAbility
{
    [SerializeField] int favourGain = 5;
    [SerializeField] HoLPlayerSet playersOnMission;
    
    public void ChoicesLockedIn()
    {
        if (playersOnMission.Value.Contains(Owner)) Owner.Favour.Value += favourGain;
    }
}
