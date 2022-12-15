using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Increment Int", menuName = "Missions/Effects/Generic/Increment Int")]
public class IncrementIntVariable : MissionEffect
{
    [Tooltip("The IntVariable to increment")]
    [SerializeField] IntVariable intVar;

    [Tooltip("How much to increment the IntVariable by")]
    [SerializeField] int incrementAmount;

    public override void TriggerEffect()
    {
        intVar.Value += incrementAmount;
        EndEffect();
    }
}
