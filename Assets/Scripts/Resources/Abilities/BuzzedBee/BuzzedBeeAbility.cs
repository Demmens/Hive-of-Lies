using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzedBeeAbility : RoleAbility
{
    DiceMission dice;
    void Start()
    {
        dice = DiceMission.singleton;
        dice.OnPlayerRolled += PlayerRolled;
    }

    void PlayerRolled(Player ply, ref PlayerRollInfo roll)
    {
        if (ply == Owner) roll.currentRoll += dice.ExhaustionPenalty * ply.Exhaustion;
    }
}
