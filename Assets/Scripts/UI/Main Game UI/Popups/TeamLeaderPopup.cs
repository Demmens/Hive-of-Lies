using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Mirror;
using Steamworks;

public class TeamLeaderPopup : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] TMPro.TMP_Text popupText;

    [SerializeField] GameObject popup;

    [Tooltip("Whether the local player is the team leader")]
    [SerializeField] BoolVariable isTeamLeader;

    [Tooltip("Whether the local player is on the mission")]
    [SerializeField] BoolVariable isOnMission;

    [Tooltip("The text for the popup when you are the Team Leader")]
    [SerializeField] LocalizedString popupText1;

    [Tooltip("The text for the popup when someone else is the Team Leader")]
    [SerializeField] LocalizedString popupText2;
    #endregion
    #region SERVER
    [Tooltip("The current team leader")]
    [SerializeField] HivePlayerVariable teamLeader;
    #endregion

    public override void OnStartServer()
    {
        teamLeader.AfterVariableChanged += leader => UnsetTeamLeader();
        teamLeader.AfterVariableChanged += leader =>
        {
            if (leader == null) return;
            SetTeamLeader(leader.connectionToClient);
        };
        teamLeader.AfterVariableChanged += leader =>
        {
            if (leader == null) return;
            LocalPlayerTeamLeaderPopup(leader.DisplayName);
        };
    }

    [ClientRpc]
    void UnsetTeamLeader()
    {
        isTeamLeader.Value = false;
        isOnMission.Value = false;
    }

    [TargetRpc]
    void SetTeamLeader(NetworkConnection conn)
    {
        isTeamLeader.Value = true;
        isOnMission.Value = true;
    }

    [Server]
    public void AfterStandOrPass()
    {
        string leaderName = teamLeader.Value == null ? "" : teamLeader.Value.DisplayName;
        LocalPlayerTeamLeaderPopup(teamLeader.Value.DisplayName);
    }

    [ClientRpc]
    void LocalPlayerTeamLeaderPopup(string leaderName)
    {

        if (isTeamLeader)
        {
            popupText.text = popupText1.GetLocalizedString();
        }
        else
        {
            popupText.text = string.Format(popupText2.GetLocalizedString(), leaderName);
        }

        popup.SetActive(true);
    }
}