using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Add Card", menuName = "Missions/Effects/Specific/Add Card to Deck")]
public class AddCardToDeck : MissionEffect
{
    [SerializeField] hivePlayerSet playerSet;
    [SerializeField] Card card;
    public override void TriggerEffect()
    {
        foreach (hivePlayer ply in playerSet.Value)
        {
            ply.Deck.Value.PublicAddToDeck(card);
            ply.Deck.Value.Shuffle();
        }
        EndEffect();
    }
}
