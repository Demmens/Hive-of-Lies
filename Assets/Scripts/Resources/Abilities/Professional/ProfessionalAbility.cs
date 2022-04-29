using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessionalAbility : RoleAbility
{
    DiceMission dice;
    void Start()
    {
        dice = DiceMission.singleton;
        dice.OnPlayerRolled += ModifyDiceRoll;
    }

    void ModifyDiceRoll(Player ply, ref PlayerRollInfo roll)
    {
        int secondRoll = Random.Range(1, 21);
        roll.currentRoll = Mathf.Max(roll.currentRoll, secondRoll);
    }
}
