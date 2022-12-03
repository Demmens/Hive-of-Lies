using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzedBeeAbility : RoleAbility
{
    private IntVariable playerExhaustion;

    void Start()
    {
        playerExhaustion = Owner.Exhaustion;
        playerExhaustion.OnVariableChanged += ExhaustionChanged;
    }

    public void ExhaustionChanged(int oldVal, ref int newVal)
    {
        newVal = oldVal;
    }
}
