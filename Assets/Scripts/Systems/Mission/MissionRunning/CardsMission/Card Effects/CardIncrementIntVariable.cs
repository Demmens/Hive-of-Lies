using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Effects/Generic/Increment Int")]
public class CardIncrementIntVariable : CardEffect
{
    [SerializeField] IntVariable var;
    [SerializeField] int value;
    public override void TriggerEffect()
    {
        var.Value += value;
    }
}
