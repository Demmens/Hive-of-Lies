using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Team Gains Favour", menuName = "Missions/Effects/Team Gains Favour")]
public class TeamGainsFavour : MissionEffect
{
    [SerializeField] int favourGain;
    [SerializeField] Team Team;

    //Effect is positive if the team is bee and favour gain is positive, or if the team is wasp and favour gain is negative. Otherwise it's a negative effect.
    public override EffectType Type => (Team == Team.Bee) == (favourGain > 0) ? EffectType.Positive : EffectType.Negative;
    public override void TriggerEffect()
    {
        foreach (Role role in GameInfo.singleton.Roles)
        {
            if (role.Data.Team == Team)
            {
                role.Ability.Owner.Favour += favourGain;
                role.Ability.Owner.Connection.Send(new ChangeFavourMsg() { favourIncrease = favourGain });
            }
        }
        EndEffect();
    }
}
