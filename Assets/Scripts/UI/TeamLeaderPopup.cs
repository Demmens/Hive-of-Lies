using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class TeamLeaderPopup : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text popupText;
    [SerializeField] GameObject popup;

    void Start()
    {
        ClientEventProvider.singleton.OnTeamLeaderChanged += Popup;
    }

    void Popup(TeamLeaderChangedMsg msg)
    {
        ClientGameInfo.singleton.TeamLeaderID = new CSteamID(msg.ID);
        ClientGameInfo.singleton.MaxPartners = msg.maxPartners;

        if (msg.ID == SteamUser.GetSteamID().m_SteamID)
        {
            popupText.text = "You are the team leader. Select the player you wish to take on the mission.";
        }
        else
        {
            popupText.text = $"{SteamFriends.GetFriendPersonaName(new CSteamID(msg.ID))} is the team leader";
        }

        popup.SetActive(true);
    }
}