using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Team Gains Favour", menuName = "Missions/Effects/Team Gains Favour")]
public class TeamGainsFavour : MissionEffect
{
    [SerializeField] int favourGain;
    [SerializeField] Team Team;

    public override void TriggerEffect()
    {
        foreach (Role role in GameInfo.singleton.Roles)
        {
            if (role.Data.Team == Team)
            {
                role.Ability.Owner.Favour.Value += favourGain;
                role.Ability.Owner.Connection.Send(new ChangeFavourMsg() { favourIncrease = favourGain });
            }
        }
        EndEffect();
    }
}
