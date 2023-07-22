using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPlayedCards : RoleAbility
{
    public void AfterDeckConstructed()
    {
        foreach (Card card in Owner.Deck.Value.DrawPile)
        {
            card.DestroyOnPlay = false;
        }
    }
}
