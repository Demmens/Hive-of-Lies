using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Investigate Player", menuName = "Missions/Effects/Specific/Investigate Player")]
public class InvestigatePlayer : MissionEffect
{
    [SerializeField] GameObject investigateButton;
    [SerializeField] GameObject notificationPrefab;
    GameObject notification;

    PlayerButtonDropdown dropDown;

    [Tooltip("All players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    [Tooltip("The current team leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

    void Awake()
    {
        NetworkClient.RegisterHandler<InvestigateEffectTriggeredMsg>(OnEffectTriggered);
        NetworkClient.RegisterHandler<InvestigateResultMsg>(GetResults);
        NetworkServer.RegisterHandler<InvestigateEffectTriggeredMsg>(OnInvestigated);
    }

    #region SERVER
    [Server]
    public override void TriggerEffect()
    {
        Debug.Log("Effect triggered");

        teamLeader.Value.Connection.Send(new InvestigateEffectTriggeredMsg() { });
    }

    /// <summary>
    /// Called when a player has been investigated. Send the client the team of the investigatee
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    [Server]
    private void OnInvestigated(NetworkConnection conn, InvestigateEffectTriggeredMsg msg)
    {
        foreach (HoLPlayer ply in allPlayers.Value)
        {
            if (ply.PlayerID == msg.playerID)
            {
                teamLeader.Value.Connection.Send(new InvestigateResultMsg()
                {
                    team = ply.Team,
                    playerName = ply.DisplayName,
                });
                //Now we have received the result, we can end the effect of the mission and continue with the game.
                EndEffect();
                return;
            }
        }
    }
    #endregion

    #region CLIENT

    [Client]
    private void OnEffectTriggered(InvestigateEffectTriggeredMsg msg)
    {
        Debug.Log("Effect triggered on client");
        //This is really bad and we shouldn't be doing this. It's currently midnight and I'm too tired to think of a better way.
        dropDown = FindObjectOfType<PlayerButtonDropdown>();
        PlayerButtonDropdownItem item = dropDown.AddAll(investigateButton);
        item.OnItemClicked += PlayerInvestigated;

        if (notification == null) notification = Instantiate(notificationPrefab);

        notification.SetActive(true);
        notification.GetComponent<InvestigatePopup>().SetText("Choose a player to investigate");
        Debug.Log("Notification created");
    }

    /// <summary>
    /// Called on the client when the player chooses who to investigate
    /// </summary>
    /// <param name="playerID"></param>
    [Client]
    public void PlayerInvestigated(ulong playerID)
    {
        dropDown.RemoveAll(investigateButton);
        InvestigateEffectTriggeredMsg msg = new InvestigateEffectTriggeredMsg()
        {
            playerID = playerID,
        };
        NetworkClient.Send(msg);
    }

    /// <summary>
    /// Called by the server to send the team of the investigated player over to the client.
    /// </summary>
    /// <param name="msg"></param>
    [Client]
    private void GetResults(InvestigateResultMsg msg)
    {
        notification.SetActive(true);
        notification.GetComponent<InvestigatePopup>().SetText($"{msg.playerName} is a {msg.team}");
    }

    #endregion
}

public struct InvestigateEffectTriggeredMsg : NetworkMessage
{
    public ulong playerID;
}

public struct InvestigateResultMsg : NetworkMessage
{
    public Team team;
    public string playerName;
}