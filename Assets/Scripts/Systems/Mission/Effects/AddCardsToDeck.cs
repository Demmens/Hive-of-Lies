using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Add Card", menuName = "Missions/Effects/Specific/Add Card to Deck")]
public class AddCardsToDeck : MissionEffect
{
    [SerializeField] HivePlayerSet playerSet;
    [SerializeField] List<Card> cards;
    public override void TriggerEffect()
    {
        foreach (HivePlayer ply in playerSet.Value)
        {
            foreach (Card card in cards) ply.Deck.Value.DrawPile.Add(Instantiate(card));
            ply.Deck.Value.Shuffle();
        }
        EndEffect();
    }
}
