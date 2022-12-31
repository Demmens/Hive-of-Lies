using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invoke Event", menuName = "Missions/Effects/Generic/Invoke Event")]
public class InvokeEvent : MissionEffect
{
    [SerializeField] GameEvent _event;
    public override void TriggerEffect()
    {
        _event?.Invoke();
    }
}
