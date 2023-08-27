using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychicAbility : RoleAbility
{
    [SerializeField] NetworkingEvent showTopCard;
    protected override void OnRoleGiven()
    {
        Owner.Deck.Value.OnDraw += (Card card) => showTopCard.Invoke(Owner.connectionToClient);
    }
}
