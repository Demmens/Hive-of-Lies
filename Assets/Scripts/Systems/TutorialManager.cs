using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TutorialManager : NetworkBehaviour
{
    [SerializeField] GameObject standOrPassTutorial;
    [SerializeField] GameObject voteTutorial;
    [SerializeField] GameObject missionTutorial;
    [SerializeField] GameObject stingTutorial;

    [SerializeField] BoolVariable hasDoneStandOrPassTutorial;
    [SerializeField] BoolVariable hasDoneVoteTutorial;
    [SerializeField] BoolVariable hasDoneMissionTutorial;
    [SerializeField] BoolVariable hasDoneStingTutorial;

    [SerializeField] BoolVariable isOnMission;
    [SerializeField] HivePlayerSet waspPlayers;
    [SerializeField] IntVariable roundNum;

    [ClientRpc]
    public void OnStandOrPassStart()
    {
        if (hasDoneStandOrPassTutorial.Value) return;
        standOrPassTutorial.SetActive(true);
        hasDoneStandOrPassTutorial.Value = true;
    }

    [Server]
    public void OnRoundTwoStart()
    {
        if (roundNum.Value != 1) return;
        
        foreach (HivePlayer ply in waspPlayers.Value)
        {
            if (ply.Target.Value == null) continue;
            StingTutorial(ply.connectionToClient);
        }
    }

    [TargetRpc]
    void StingTutorial(NetworkConnection conn)
    {
        if (hasDoneStingTutorial.Value) return;
        stingTutorial.SetActive(true);
        hasDoneStingTutorial.Value = true;
    }

    [ClientRpc]
    public void OnVoteStart()
    {
        if (hasDoneVoteTutorial.Value) return;
        voteTutorial.SetActive(true);
        hasDoneVoteTutorial.Value = true;
    }

    [Client]
    public void OnVoteResultPopupClosed()
    {
        if (hasDoneMissionTutorial.Value) return;
        if (!isOnMission.Value) return;
        missionTutorial.SetActive(true);
        hasDoneMissionTutorial.Value = true;
    }
}
