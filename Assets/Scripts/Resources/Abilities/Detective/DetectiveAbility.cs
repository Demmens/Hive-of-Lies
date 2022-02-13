using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetectiveAbility : RoleAbility
{
    /// <summary>
    /// The round on which the ability triggers
    /// </summary>
    [SerializeField] int abilityTriggerRound = 4;

    /// <summary>
    /// Reference to the information popup
    /// </summary>
    [SerializeField] GameObject popup;

    [SerializeField] TMP_Text text;

    RunMission mission;
    GameInfo info;

    void Start()
    {
        if (!Active) return;
        mission = FindObjectOfType<RunMission>();
        info = FindObjectOfType<GameInfo>();
        mission.OnGamePhaseChange += new GamePhase.GamePhaseChange(() =>
        {
            if (GameInfo.RoundNum == abilityTriggerRound)
            {
                string txt = "";
                info.Roles.Shuffle();
                foreach(Role role in info.Roles)
                {
                    if (role.Data.Team == Team.Bee)
                    {
                        txt = $"{role.Ability.Owner.DisplayName} is the {role.Data.Name}";
                        break;
                    }
                }
                text.text = txt;
                //Enable popup on client
                //if (client)
                popup.SetActive(true);
                //
            }
        });
    }
}
