using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    #endregion
    #region SERVER
    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;
    #endregion

    public override void OnStartServer()
    {
        teamLeader.AfterVariableChanged += leader => UnsetTeamLeader();
        teamLeader.AfterVariableChanged += leader =>
        {
            if (leader == null) return;
            SetTeamLeader(leader.Connection);
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
        LocalPlayerTeamLeaderPopup(teamLeader.Value.DisplayName);
    }

    [ClientRpc]
    void LocalPlayerTeamLeaderPopup(string leaderName)
    {

        if (isTeamLeader)
        {
            popupText.text = "You are the team leader. Select the player you wish to take on the mission.";
        }
        else
        {
            popupText.text = $"{leaderName} is the team leader";
        }

        popup.SetActive(true);
    }
}