using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tooltip("Guarantee a given card to appear after a given number of draws")]
public class GuaranteeCard : RoleAbility
{
    [Tooltip("The amount of draws it takes to get the guaranteed card")]
    [SerializeField] int drawNum;

    [Tooltip("The card that is guaranteed")]
    [SerializeField] Card card;

    protected override void OnRoleGiven()
    {
        Owner.Deck.Value.BeforeDraw += OnDraw;
    }

    private void OnDraw(ref Card draw, bool simulated)
    {
        if (Owner.NumDraws + 1 != drawNum) return;

        draw = card;
    }
}
