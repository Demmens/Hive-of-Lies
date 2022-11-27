using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Mercenary Ability", menuName = "Roles/Abilities/Bee/Mercenary")]
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
        /*if (GameInfo.singleton.PlayersOnMission.Contains(Owner.ID))
        {
            Owner.Favour += favourGain;
            OwnerConnection.Send(new ChangeFavourMsg() { favourIncrease = favourGain });
        }*/
    }
}
