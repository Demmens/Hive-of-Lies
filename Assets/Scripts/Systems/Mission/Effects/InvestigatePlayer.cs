using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InvestigatePlayer : MissionEffectBehaviour
{
    [SerializeField] GameObject investigateButton;
    [SerializeField] GameObject notificationPrefab;
    GameObject notification;

    PlayerButtonDropdown dropDown;
    bool isResult = false;

    [Tooltip("All players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

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
    }

    [Client]
    public override void OnStartAuthority()
    {
        //This is really bad and we shouldn't be doing this. It's currently midnight and I'm too tired to think of a better way.
        dropDown = PlayerButtonDropdown.singleton;
        PlayerButtonDropdownItem item = dropDown.AddAll(investigateButton);
        item.OnItemClicked += PlayerInvestigated;

        notification = Instantiate(notificationPrefab);

        notification.GetComponent<Notification>().SetText("Choose a player to investigate");

        notification = null;
    }

    /// <summary>
    /// Called on the client when the player chooses who to investigate
    /// </summary>
    /// <param name="playerID"></param>
    [Client]
    public void PlayerInvestigated(ulong playerID)
    {
        dropDown.RemoveAll(investigateButton);
        OnInvestigated(playerID);
    }

    /// <summary>
    /// Called when a player has been investigated. Send the client the team of the investigatee
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    [Command]
    private void OnInvestigated(ulong playerID)
    {
        foreach (HoLPlayer ply in allPlayers.Value)
        {
            if (ply.PlayerID == playerID)
            {
                GetResults(ply.DisplayName, ply.Team);
                return;
            }
        }
    }

    /// <summary>
    /// Called by the server to send the team of the investigated player over to the client.
    /// </summary>
    /// <param name="msg"></param>
    [TargetRpc]
    private void GetResults(string playerName, Team team)
    {
        notification = Instantiate(notificationPrefab);

        notification.GetComponent<Notification>().SetText($"{playerName} is a {team}");
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