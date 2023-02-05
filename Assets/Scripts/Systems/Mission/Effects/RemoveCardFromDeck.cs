using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Remove Card", menuName = "Missions/Effects/Specific/Remove Card from Deck")]
public class RemoveCardFromDeck : MissionEffect
{
    [SerializeField] HoLPlayerSet playerSet;
    [SerializeField] int cardValue;
    public override void TriggerEffect()
    {
        foreach (HoLPlayer ply in playerSet.Value)
        {
            foreach (Card card in ply.Deck.Value.DrawPile)
            {
                if (card.Value == cardValue)
                {
                    ply.Deck.Value.DrawPile.Remove(card);
                    return;
                }
            }
        }
    }
}
