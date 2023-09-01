using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CaptainAbility : RoleAbility
{
    [SerializeField] HivePlayerVariable teamLeader;
    [SerializeField] HivePlayerSet allPlayers;

    [Server]
    protected override void OnRoleGiven()
    {
        //Just apply the debuff to everyone, and make the debuff not do anything if we're not team leader
        foreach (HivePlayer ply in allPlayers.Value)
        {
            if (ply == Owner) continue;
            ply.Deck.Value.BeforeDraw += (ref Card card, bool simulated) => PlayerDrew(ply, ref card, simulated);
        };
    }

    void PlayerDrew(HivePlayer ply, ref Card card, bool simulated)
    {
        if (teamLeader.Value != Owner) return;
        Deck deck = ply.Deck;

        if (deck.DrawPile.Count < 2) return;

        //If the card has been messed with already, leave it alone
        if (card != deck.DrawPile[0]) return;

        int cardToPlaceOnBottom = 1;

        //If the second card is a better draw than the first
        if (deck.DrawPile[1].Value > deck.DrawPile[0].Value)
        {
            card = deck.DrawPile[1];
            //Place the first card at the bottom of the draw pile
            cardToPlaceOnBottom = 0;
        }

        //If we aren't actually drawing a card - just trying to find out what card we would draw, we don't want to actually modify the deck.
        if (simulated) return;

        deck.DrawPile.Add(deck.DrawPile[cardToPlaceOnBottom]);
        deck.DrawPile.RemoveAt(cardToPlaceOnBottom);

    }
}
