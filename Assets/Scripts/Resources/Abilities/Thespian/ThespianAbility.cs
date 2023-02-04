using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ThespianAbility : RoleAbility
{
    [SerializeField] int favourGain = 5;
    [SerializeField] HoLPlayerSet playersOnMission;
    [SerializeField] VoteSet votes;

    public void ChoicesLockedIn()
    {
        if (votes.Value.Count(vote => vote.votes < 0) > 0) return;
        if (!playersOnMission.Value.Contains(Owner)) return;
            
        Owner.Favour.Value += favourGain;
    }
}
