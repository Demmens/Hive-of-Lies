using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WaspsGainFavour : MissionEffect
{
    [SerializeField] int favourGain;

    public override EffectType Type => favourGain > 0 ? EffectType.Negative : EffectType.Positive;
    public override void TriggerEffect()
    {
        foreach (Role role in GameInfo.singleton.Roles)
        {
            if (role.Data.Team == Team.Wasp)
            {
                role.Ability.Owner.Favour += favourGain;
                role.Ability.Owner.Connection.Send(new ChangeFavourMsg() { favourIncrease = favourGain });
            }
        }
        EndEffect();
    }
}
