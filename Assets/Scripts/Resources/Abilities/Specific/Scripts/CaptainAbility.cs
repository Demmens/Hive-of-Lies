using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CaptainAbility : RoleAbility
{
    [SerializeField] HoLPlayerVariable teamLeader;
    [SerializeField] HoLPlayerSet allPlayers;

    [Server]
    protected override void OnRoleGiven()
    {
        //Just apply the debuff to everyone, and make the debuff not do anything if we're not team leader
        foreach (HoLPlayer ply in allPlayers.Value)
        {
            if (ply == Owner) continue;
            ply.Deck.Value.BeforeDraw += (ref Card card) => PlayerDrew(ply, ref card);
        };
    }

    void PlayerDrew(HoLPlayer ply, ref Card card)
    {
        if (teamLeader.Value != Owner) return;
        Deck deck = ply.Deck;

        if (deck.DrawPile.Count < 2) return;

        int cardToPlaceOnBottom = 1;

        //If the second card is a better draw than the first
        if (deck.DrawPile[1].TempValue > deck.DrawPile[0].TempValue)
        {
            card = deck.DrawPile[1];
            //Place the first card at the bottom of the draw pile
            cardToPlaceOnBottom = 0;
        }
  
        deck.DrawPile.Add(deck.DrawPile[cardToPlaceOnBottom]);
        deck.DrawPile.RemoveAt(cardToPlaceOnBottom);

    }
}
