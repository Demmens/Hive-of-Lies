using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "CardInfo", menuName = "Variable/Dictionaries/HoLPlayer -> Deck")]
public class CardInfo : Variable<Dictionary<HivePlayer, Deck>>
{
}