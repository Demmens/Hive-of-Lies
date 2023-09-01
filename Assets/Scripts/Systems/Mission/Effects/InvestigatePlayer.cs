using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Mirror;

public class InvestigatePlayer : MissionEffectBehaviour
{
    [SerializeField] GameObject investigateButton;
    [SerializeField] GameObject notificationPrefab;
    GameObject notification;

    [Tooltip("All players in the game")]
    [SerializeField] HivePlayerSet allPlayers;

    [Tooltip("The current team leader")]
    [SerializeField] HivePlayerVariable teamLeader;

    private List<PlayerButtonDropdownItem> buttons = new();

    [SerializeField] LocalizedString promptText;
    [SerializeField] LocalizedString resultText;

    [Server]
    public override void OnEffectTriggered()
    {
        //If there's no team leader, quit early
        if (teamLeader.Value == null)
        {
            EndEffect();
            return;
        }

        //Team leader has authority over this object
        netIdentity.AssignClientAuthority(teamLeader.Value.connectionToClient);

        foreach (HivePlayer ply in allPlayers.Value)
        {
            PlayerButtonDropdownItem item = ply.Button.AddDropdownItem(investigateButton, teamLeader);
            buttons.Add(item);
            item.OnItemClicked += OnInvestigated;
        }
    }

    [Client]
    public override void OnStartAuthority()
    {
        notification = Instantiate(notificationPrefab);

        notification.GetComponent<Notification>().SetText(promptText.GetLocalizedString());

        notification = null;
    }

    /// <summary>
    /// Called when a player has been investigated. Send the client the team of the investigatee
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    [Server]
    private void OnInvestigated(HivePlayer ply)
    {
        foreach (PlayerButtonDropdownItem button in buttons)
        {
            Destroy(button);
        }
        GetResults(ply.DisplayName, ply.Team);
    }

    /// <summary>
    /// Called by the server to send the team of the investigated player over to the client.
    /// </summary>
    /// <param name="msg"></param>
    [TargetRpc]
    private void GetResults(string playerName, ETeam team)
    {
        notification = Instantiate(notificationPrefab);

        notification.GetComponent<Notification>().SetText(string.Format(resultText.GetLocalizedString(), playerName, team));
        notification.GetComponent<Notification>().OnNotificationClosed += OnClosedPopup;

        notification = null;
    }

    [Command]
    private void OnClosedPopup()
    {
        //Only continue with the game when the team leader has closed the result popup
        EndEffect();
    }
}