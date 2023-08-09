using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TutorialManager : NetworkBehaviour
{
    [SerializeField] GameObject standOrPassTutorial;
    [SerializeField] GameObject voteTutorial;
    [SerializeField] GameObject missionTutorial;

    [SerializeField] BoolVariable hasDoneStandOrPassTutorial;
    [SerializeField] BoolVariable hasDoneVoteTutorial;
    [SerializeField] BoolVariable hasDoneMissionTutorial;

    [SerializeField] BoolVariable isOnMission;

    [ClientRpc]
    public void OnStandOrPassStart()
    {
        if (hasDoneStandOrPassTutorial.Value) return;
        standOrPassTutorial.SetActive(true);
        hasDoneStandOrPassTutorial.Value = true;
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
