using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FumbleBeeAbility : RoleAbility
{
    /*DiceMission dice;
    /// <summary>
    /// The number your roll is capped at
    /// </summary>
    [SerializeField] int rollCap = 5;
    /// <summary>
    /// The number of rolls you make with this cap enabled
    /// </summary>
    [SerializeField] int rollsWithCap = 2;
    void Start()
    {
        dice = FindObjectOfType<DiceMission>();

        dice.OnPlayerRolled += ModifyDiceRoll;
    }

    void ModifyDiceRoll(Player ply, ref PlayerRollInfo roll)
    {
        if (ply != Owner || roll.rerollsUsed < rollsWithCap || roll.currentRoll <= rollCap) return;

        roll.currentRoll = Random.Range(1, rollCap);
    }*/
}
