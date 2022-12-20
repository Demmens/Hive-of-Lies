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
    #endregion
    #region SERVER
    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;
    #endregion

    public override void OnStartServer()
    {
        teamLeader.AfterVariableChanged += leader => SetTeamLeader(leader.Connection);
        teamLeader.AfterVariableChanged += leader => LocalPlayerTeamLeaderPopup(leader.DisplayName);
    }

    [TargetRpc]
    void SetTeamLeader(NetworkConnection conn)
    {
        isTeamLeader.Value = true;
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