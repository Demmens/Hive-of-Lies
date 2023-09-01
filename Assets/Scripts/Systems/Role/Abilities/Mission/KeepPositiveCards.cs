using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPositiveCards : RoleAbility
{
    public void AfterDeckConstructed()
    {
        foreach (Card card in Owner.Deck.Value.DrawPile)
        {
            if (card.Value > 0) card.DestroyOnPlay = false;
        }
    }
}