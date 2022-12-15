using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Change Bool", menuName = "Missions/Effects/Generic/Change Bool")]
public class ChangeBoolVariable : MissionEffect
{
    [Tooltip("The BoolVariable to change")]
    [SerializeField] BoolVariable boolVar;

    [Tooltip("What to change the BoolVariable to")]
    [SerializeField] bool change;

    public override void TriggerEffect()
    {
        boolVar.Value = change;
        EndEffect();
    }
}
