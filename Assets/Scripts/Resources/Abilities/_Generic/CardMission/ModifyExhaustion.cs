using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyExhaustion : RoleAbility
{
    [SerializeField] IntVariable exhaustionPenalty;
    [SerializeField] float modifier;

    protected override void OnRoleGiven()
    {
        Owner.Deck.Value.OnDraw += ExhaustionChanged;
    }

    public void ExhaustionChanged(Card card)
    {
        card.TempValue += Mathf.FloorToInt(exhaustionPenalty * Owner.Exhaustion * (modifier - 1));
    }
}
