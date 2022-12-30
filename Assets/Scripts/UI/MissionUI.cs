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
    [SerializeField] TMP_Text successFlavour;
    [SerializeField] TMP_Text successEffect;
    [SerializeField] TMP_Text failFlavour;
    [SerializeField] TMP_Text failEffect;

    [SerializeField] TMP_Text teamLeaderName;
    [SerializeField] TMP_Text missionPlayerList;
    [SerializeField] TMP_Text missionCost;
    #endregion

    #region SERVER
    [Tooltip("The mission that is currently active")]
    [SerializeField] MissionVariable currentMission;

    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

    [Tooltip("All players that have currently been selected to go on the mission")]
    [SerializeField] HoLPlayerSet playersSelected;

    [Tooltip("Returns true if the player is on the mission")]
    [SerializeField] BoolVariable isOnMission;

    [Tooltip("The ID of the local player")]
    [SerializeField] UlongVariable id;
    #endregion

    public override void OnStartServer()
    {
        currentMission.AfterVariableChanged += ChangeMission;
        teamLeader.AfterVariableChanged += OnTeamLeaderDecided;
        playersSelected.AfterItemAdded += OnTeamLeaderAddPartner;
        playersSelected.AfterItemRemoved += OnTeamLeaderRemovePartner;
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
    void ChangeMission(Mission mission)
    {
        missionUI.SetActive(true);
        missionName.text = mission.MissionName;
        missionFlavour.text = mission.Description;
        successFlavour.text = mission.SuccessFlavour;
        failFlavour.text = mission.FailFlavour;
        missionCost.text = $"Mission Cost: {mission.FavourCost}f";
        missionPlayerList.text = "Undecided";

        successEffect.text = CreateStringFromList(mission.SuccessEffects);
        failEffect.text = CreateStringFromList(mission.FailEffects);
    }

    /// <summary>
    /// Called when the team leader is decided
    /// </summary>
    /// <param name="msg"></param>
    [ClientRpc]
    void OnTeamLeaderDecided(HoLPlayer ply)
    {
        teamLeaderName.text = ply.DisplayName;
    }

    [ClientRpc]
    void OnTeamLeaderAddPartner(HoLPlayer ply)
    {
        //If we haven't selected any players yet, clear the text, otherwise add a line break for the next player
        missionPlayerList.text = (missionPlayerList.text == "Undecided") ? "" : missionPlayerList.text + "\n";

        missionPlayerList.text += ply.DisplayName;

        if (ply.PlayerID == (ulong) SteamUser.GetSteamID()) isOnMission.Value = true;
    }

    [ClientRpc]
    void OnTeamLeaderRemovePartner(HoLPlayer ply)
    {
        missionPlayerList.text = missionPlayerList.text.Replace(ply.DisplayName, "");
        //Make sure to remove duplicate line breaks
        missionPlayerList.text = missionPlayerList.text.Replace("\n\n", "\n");
        if (missionPlayerList.text == "" || missionPlayerList.text == "\n") missionPlayerList.text = "Undecided";
    }
}