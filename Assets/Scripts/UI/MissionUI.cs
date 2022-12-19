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
        ClientGameInfo.singleton.CurrentlySelected = new List<ulong>();
        missionUI.SetActive(true);
        missionName.text = mission.MissionName;
        missionFlavour.text = mission.Description;
        successFlavour.text = mission.SuccessFlavour;
        failFlavour.text = mission.FailFlavour;
        missionCost.text = $"Mission Cost: {mission.FavourCost}f";
        teamLeaderName.text = "Undecided";
        missionPlayerList.text = "Undecided";

        successEffect.text = CreateStringFromList(mission.SuccessEffects);
        failEffect.text = CreateStringFromList(mission.FailEffects);
    }

    /// <summary>
    /// Called when the team leader is decided
    /// </summary>
    /// <param name="msg"></param>
    void OnTeamLeaderDecided(TeamLeaderChangedMsg msg)
    {
        teamLeaderName.text = SteamFriends.GetFriendPersonaName(new CSteamID(msg.ID));
    }

    /// <summary>
    /// Called when the team leader has added or removed a player from the mission.
    /// </summary>
    /// <param name="msg"></param>
    void OnTeamLeaderChangePartner(TeamLeaderChangePartnersMsg msg)
    {
        Debug.Log("Team Leader has changed partners");
        string playerName = SteamFriends.GetFriendPersonaName(msg.playerID);

        if (!ClientGameInfo.singleton.CurrentlySelected.Contains((ulong) msg.playerID)) ClientGameInfo.singleton.CurrentlySelected.Add((ulong) msg.playerID);

        if (msg.selected)
        {
            //If we haven't selected any players yet, clear the text, otherwise add a line break for the next player
            missionPlayerList.text = (missionPlayerList.text == "Undecided") ? "" : missionPlayerList.text + "\n";

            missionPlayerList.text += playerName;
        }
        else
        {
            missionPlayerList.text = missionPlayerList.text.Replace(playerName, "");
            //Make sure to remove duplicate line breaks
            missionPlayerList.text = missionPlayerList.text.Replace("\n\n", "\n");
            if (missionPlayerList.text == "" || missionPlayerList.text == "\n") missionPlayerList.text = "Undecided";
        }
    }
}