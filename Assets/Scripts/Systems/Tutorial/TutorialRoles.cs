using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class TutorialRoles : NetworkBehaviour
{
    #region SERVER
    [Tooltip("The set of all players")]
    [SerializeField] HivePlayerSet players;
    #endregion

    #region CLIENT
    [SerializeField] GameObject teamScreen;
    [SerializeField] TMP_Text teamNameText;
    [SerializeField] TMP_Text roleNameText;
    [SerializeField] TMP_Text roleDescriptionText;
    #endregion
    public override void OnStartServer()
    {
        foreach (HivePlayer ply in players)
        {
            if (ply.Team.Value == null) ply.Team.AfterVariableChanged += (team) => OnTeamChanged(ply.connectionToClient, team.Name_indef);
            else OnTeamChanged(ply.connectionToClient, ply.Team.Value.Name_indef);

            ply.Role.AfterVariableChanged += (role) => { if (role != null) OnRoleChanged(ply.connectionToClient, role.Data); };
        }
    }

    [TargetRpc]
    void OnRoleChanged(NetworkConnection conn, RoleData data)
    {
        if (data == null) return;
        roleNameText.text = data.RoleName;
        roleDescriptionText.text = data.Description;
    }

    void OnTeamChanged(NetworkConnection conn, string teamName)
    {
        teamNameText.text = teamName;
        teamScreen.SetActive(true);
    }
}
