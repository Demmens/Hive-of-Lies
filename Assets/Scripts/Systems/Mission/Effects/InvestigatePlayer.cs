using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Investigate Player", menuName = "Missions/Effects/Specific/Investigate Player")]
public class InvestigatePlayer : MissionEffect
{
    [SerializeField] GameObject investigateButton;

    public static InvestigatePlayer Singleton;
    void Start()
    {
        Singleton = this;
        NetworkClient.RegisterHandler<InvestigateEffectTriggeredMsg>(OnEffectTriggered);
        NetworkClient.RegisterHandler<InvestigateResultMsg>(GetResults);
        NetworkServer.RegisterHandler<InvestigateEffectTriggeredMsg>(OnInvestigated);
    }

    #region SERVER
    public override void TriggerEffect()
    {
        GameInfo.singleton.TeamLeader.Connection.Send(new InvestigateEffectTriggeredMsg() { });
    }

    /// <summary>
    /// Called when a player has been investigated. Send the client the team of the investigatee
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void OnInvestigated(NetworkConnection conn, InvestigateEffectTriggeredMsg msg)
    {
        foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.singleton.Players)
        {
            if (pair.Value.ID == msg.playerID)
            {
                GameInfo.singleton.TeamLeader.Connection.Send(new InvestigateResultMsg()
                {
                    team = pair.Value.Team,
                    playerName = pair.Value.DisplayName,
                });
                //Now we have received the result, we can end the effect of the mission and continue with the game.
                EndEffect();
                return;
            }
        }
    }
    #endregion

    #region CLIENT

    private void OnEffectTriggered(InvestigateEffectTriggeredMsg msg)
    {
        PlayerButtonDropdown.singleton.AddItem(investigateButton);
        Notification.Singleton.CreateNotification($"Choose a player to investigate");
    }

    /// <summary>
    /// Called on the client when the player chooses who to investigate
    /// </summary>
    /// <param name="playerID"></param>
    public void PlayerInvestigated(ulong playerID)
    {
        PlayerButtonDropdown.singleton.RemoveItem(investigateButton);
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
    private void GetResults(InvestigateResultMsg msg)
    {
        Notification.Singleton.CreateNotification($"{msg.playerName} is a {msg.team}");
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