using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buzzed Bee Ability", menuName = "Roles/Abilities/Bee/Buzzed Bee")]
public class BuzzedBeeAbility : RoleAbility
{
    CardsMission cards;

    List<Card> affectedCards = new List<Card>();
    void Start()
    {
        cards = CardsMission.singleton;
        cards.OnPlayCard += PlayerPlayed;
        cards.OnDrawCard += PlayerDrew;
        RunMission.singleton.OnMissionEnd += MissionEnded;
    }

    void PlayerDrew(Player ply, ref Card card)
    {
        //if (ply == Owner)
        {
            card.DisplayValue += CardsMission.ExhaustionPenalty * ply.Exhaustion;
            affectedCards.Add(card);
        }
    }

    void PlayerPlayed(Player ply, ref Card card, ref int value)
    {
        //if (ply == Owner)
        {
            value += CardsMission.ExhaustionPenalty * ply.Exhaustion;
        }
    }

    void MissionEnded(MissionResult result)
    {
        //affectedCards.ForEach((Card card) => { card.DisplayValue -= CardsMission.ExhaustionPenalty * Owner.Exhaustion; });
        affectedCards = new List<Card>();
    }
}
