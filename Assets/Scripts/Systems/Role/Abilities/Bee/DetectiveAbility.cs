using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

[CreateAssetMenu(fileName = "Detective Ability", menuName = "Roles/Abilities/Bee/Detective")]
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
        mission = FindObjectOfType<RunMission>();
        mission.OnGamePhaseEnd += () =>
        {
            if (GameInfo.singleton.RoundNum == abilityTriggerRound)
            {
                string txt = "";
                GameInfo.singleton.Roles.Shuffle();
                foreach (Role role in GameInfo.singleton.Roles)
                {
                    if (role.Team == Team.Bee && !role.Abilities.Contains(this))
                    {
                        //txt = $"{role.Ability.Owner.DisplayName} is the {role.Data.RoleName}";
                        break;
                    }
                }
                text.text = txt;
                //SpawnPopup(Owner.Connection);
            }
        };
    }

    [TargetRpc]
    void SpawnPopup(NetworkConnection target)
    {
        popup.SetActive(true);
    }
}
