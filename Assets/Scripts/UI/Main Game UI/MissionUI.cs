using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;

public class MissionUI : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] GameObject missionUI;
    [SerializeField] TMP_Text missionName;
    [SerializeField] TMP_Text missionFlavour;

    [SerializeField] List<GameObject> effects;
    [SerializeField] List<TMP_Text> effectTexts;
    [SerializeField] List<TMP_Text> effectRequirements;
    [SerializeField] List<GameObject> separators;

    [Tooltip("Returns true if the player is on the mission")]
    [SerializeField] BoolVariable isOnMission;
    #endregion

    #region SERVER

    [Tooltip("How much more difficult all missions are")]
    [SerializeField] IntVariable missionDifficulty;

    [Tooltip("The mission that is currently active")]
    [SerializeField] MissionVariable currentMission;

    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

    [Tooltip("All players that have currently been selected to go on the mission")]
    [SerializeField] HoLPlayerSet playersSelected;
    #endregion

    public override void OnStartServer()
    {
        currentMission.AfterVariableChanged += miss => ChangeMission(miss, missionDifficulty);
        teamLeader.AfterVariableChanged += ply =>
        {
            if (ply == null) return;
            GiveTeamLeader(ply.connectionToClient);
        };
        playersSelected.AfterItemAdded += ply => OnPartnerChange(ply.DisplayName, ply.PlayerID, true);
        playersSelected.AfterItemRemoved += ply => OnPartnerChange(ply.DisplayName, ply.PlayerID, false);
    }

    /// <summary>
    /// Creates a string from a list of mission effects
    /// </summary>
    /// <param name="list">The list of mission effects</param>
    /// <returns></returns>
    public static string CreateStringFromList(List<MissionEffect> list)
    {
        string res = "";
        for (int i = 0; i < list.Count; i++)
        {
            res += list[i].Description;
            if (i != list.Count - 1)
            {
                res += ", ";
            }
        }
        if (res == "") res = "No effect";
        return res;
    }

    [ClientRpc]
    void ChangeMission(Mission mission, int difficultyMod)
    {
        if (mission == null) return;
        missionUI.SetActive(true);
        missionName.text = mission.MissionName;
        missionFlavour.text = mission.Description;

        for (int i = 0; i < effects.Count; i++) {

            if (i >= mission.effects.Count)
            {
                effects[i].SetActive(false);
                if (i > 0) separators[i-1].SetActive(false);
                continue;
            }

            effects[i].SetActive(true);
            if (i > 0) separators[i-1].SetActive(true);

            MissionEffectTier tier = mission.effects[i];

            string effectText = "";

            foreach (MissionEffect eff in tier.effects)
            {
                effectText += eff.Description + "\n";
            }

            foreach (EMissionPlotPoint point in tier.plotPoints)
            {
                if (point.Description == "") continue;
                effectText += point.Description + "\n";
            }

            //If there are no mission effects
            if (effectText == "") effectText = "No Effect";
            //Otherwise we can remove the last line break
            else effectText.TrimEnd('\n');

            effectTexts[i].text = effectText;

            if (i > 0) effectRequirements[i].text = (tier.Value + difficultyMod + mission.DifficultyMod).ToString();
        }
    }

    //TODO: Move this to a more appropriate place
    [TargetRpc]
    void GiveTeamLeader(NetworkConnection conn)
    {
        isOnMission.Value = true;
    }

    //TODO: Move this to a more appropriate place
    [ClientRpc]
    void OnPartnerChange(string name, ulong ID, bool added)
    {
        if (ID != (ulong)SteamUser.GetSteamID()) return;
        isOnMission.Value = added;
    }
}