using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CEOAbility : RoleAbility
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

        int cardToDraw = 1;

        if (deck.DrawPile[1].TempValue < deck.DrawPile[0].TempValue)
        {
            card = deck.DrawPile[1];
            cardToDraw = 0;
        }
  
        deck.DiscardPile.Add(deck.DrawPile[cardToDraw]);
        deck.DrawPile.RemoveAt(cardToDraw);

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
