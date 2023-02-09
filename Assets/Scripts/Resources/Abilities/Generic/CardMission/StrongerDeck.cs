using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongerDeck : RoleAbility
{
    [SerializeField] int deckStrengthModifier;
    [SerializeField] int maxCardValue = 20;
    public void AfterDeckConstructed()
    {
        foreach (Card card in Owner.Deck.Value.DrawPile)
        {
            card.Value = Mathf.Min(maxCardValue, card.Value + deckStrengthModifier);
        }
    }
}
