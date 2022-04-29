using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

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

    void Start()
    {
        if (!Active) return;
        mission = FindObjectOfType<RunMission>();
        mission.OnGamePhaseEnd += () =>
        {
            if (GameInfo.RoundNum == abilityTriggerRound)
            {
                string txt = "";
                GameInfo.Roles.Shuffle();
                foreach (Role role in GameInfo.Roles)
                {
                    if (role.Data.Team == Team.Bee && !(role.Ability is DetectiveAbility))
                    {
                        txt = $"{role.Ability.Owner.DisplayName} is the {role.Data.RoleName}";
                        break;
                    }
                }
                text.text = txt;
                SpawnPopup(Owner.connection);
            }
        };
    }

    [TargetRpc]
    void SpawnPopup(NetworkConnection target)
    {
        popup.SetActive(true);
    }
}
