using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Investigate : MissionEffect
{
    [SerializeField] GameObject dropDownPrefab;
    [SerializeField] GameObject textPrefab;
    [SerializeField] GameObject resultPrefab;

    public Player instigator;
    void Start()
    {
        NetworkClient.RegisterHandler<SendInvestigateMsg>(ReceiveInvestigateResult);
        NetworkServer.RegisterHandler<SendInvestigateMsg>(PlayerInvestigated);
        NetworkClient.RegisterHandler<InvestigateStartedMsg>(InvestigateStarted);
    }

    #region Server
    public override void TriggerEffect()
    {
        instigator = GameInfo.singleton.TeamLeader;
        PlayerButtonDropdown.singleton.AddItem(dropDownPrefab);
        instigator.Connection.Send(new InvestigateStartedMsg() { });
    }


    private void PlayerInvestigated(NetworkConnection conn, SendInvestigateMsg msg)
    {
        PlayerButtonDropdown.singleton.RemoveItem(dropDownPrefab);

        string result = "";

        foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.singleton.Players)
        {
            if (pair.Value.ID == msg.playerID)
            {
                result = pair.Value.Team.ToString();
            }
        }

        instigator.Connection.Send(new SendInvestigateMsg()
        {
            result = result
        });

        EndEffect();
    }

    #endregion

    #region Client
    private void InvestigateStarted(InvestigateStartedMsg msg)
    {
        textPrefab = Instantiate(textPrefab);
    }

    private void ReceiveInvestigateResult(SendInvestigateMsg msg)
    {
        Destroy(textPrefab);
        Instantiate(resultPrefab);
    }
    #endregion

    public struct SendInvestigateMsg : NetworkMessage
    {
        public ulong playerID;
        public string result;
    }

    public struct InvestigateStartedMsg : NetworkMessage
    {

    }
}