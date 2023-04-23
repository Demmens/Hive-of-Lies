using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Effects/Generic/Invoke Event")]
public class CardInvokeEvent : CardEffect
{
    [SerializeField] GameEvent ev;
    public override void TriggerEffect()
    {
        ev?.Invoke();
    }
}
