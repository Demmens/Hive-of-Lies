using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamblerAbility : RoleAbility
{
    DiceMission dice;

    List<int> previousRolls;
    bool goneBust = false;

    void Start()
    {
        dice = FindObjectOfType<DiceMission>();

        dice.OnRerollCaculation += new DiceMission.RerollCalculation(ModifyRerollCost);
        dice.OnPlayerRolled += new DiceMission.PlayerRolled(PlayerRolled);
        dice.OnMissionEnded += new MissionType.MissionEnded(() => 
        {
            previousRolls = new List<int>();
            goneBust = false;
        });
    }

    void ModifyRerollCost(Player ply, ref int cost)
    {
        if (ply != Owner || !Active) return;

        cost = goneBust ? 99 : 0;
    }

    void PlayerRolled(Player ply, PlayerRollInfo roll)
    {
        if (ply != Owner || !Active) return;

        if (previousRolls.Contains(roll.currentRoll))
        {
            GoBust();
            return;
        }

        previousRolls.Add(roll.currentRoll);
    }

    void GoBust()
    {
        goneBust = true;
    }
}
