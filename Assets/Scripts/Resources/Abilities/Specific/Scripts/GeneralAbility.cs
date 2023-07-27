using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GeneralAbility : RoleAbility
{
    [SerializeField] HoLPlayerVariable teamLeader;
    [SerializeField] HoLPlayerSet playersOnMission;

    [Server]
    public void OnMissionStarted()
    {
        if (Owner != teamLeader.Value) return;

        foreach (HoLPlayer ply in playersOnMission.Value)
        {
            if (ply == Owner) continue;
            ply.Deck.Value.BeforeDraw += (ref Card card) => PlayerDrew(ply, ref card);
        };
    }

    void PlayerDrew(HoLPlayer ply, ref Card card)
    {
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

    public void OnMissionEnded()
    {
        if (Owner != teamLeader.Value) return;

        foreach (HoLPlayer ply in playersOnMission.Value)
        {
            if (ply == Owner) continue;
            ply.Deck.Value.BeforeDraw -= (ref Card card) => PlayerDrew(ply, ref card);
        };
    }
}
