using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MercenaryAbility : RoleAbility
{
    [SerializeField] int favourGain = 5;
    TeamLeaderPickPartners pick;
    void Start()
    {
        pick = FindObjectOfType<TeamLeaderPickPartners>();
        pick.OnLockInChoices += ChoicesLockedIn;
    }

    void ChoicesLockedIn()
    {
        if (GameInfo.PlayersOnMission.Contains(Owner))
        {
            Owner.Favour += favourGain;
            Owner.Connection.Send(new ChangeFavourMsg() { favourIncrease = favourGain });
        }
    }
}
