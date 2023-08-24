using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class GamblerAbility : RoleAbility
{
    #region CLIENT
    [SerializeField] GameObject successButton;
    [SerializeField] GameObject failButton;
    #endregion
    #region SERVER
    bool votedForSuccess;
    bool votedForFail;

    [SerializeField] MissionResultVariable result;
    [SerializeField] IntVariable voteTotal;
    [SerializeField] int favourGain;
    #endregion

    [Client]
    public override void OnStartAuthority()
    {
        successButton = Instantiate(successButton);
        GenericButton button = successButton.GetComponent<GenericButton>();
        button.SetText("Next mission will succeed");
        button.SetPos(new Vector3((Screen.width / 2) - 200, 80, 0));
        button.OnClicked += () => OnClicked(true);
        successButton.SetActive(false);

        failButton = Instantiate(failButton);
        button = failButton.GetComponent<GenericButton>();
        button.SetText("Next mission will fail");
        button.SetPos(new Vector3((Screen.width / 2) + 200, 80, 0));
        button.OnClicked += () => OnClicked(false);
        failButton.SetActive(false);
    }

    [Server]
    public void AfterVoteResult()
    {
        //If the vote didn't go through, don't display buttons
        if (voteTotal <= 0) return;
        votedForSuccess = false;
        votedForFail = false;
        SetButtonsActive();
    }

    [TargetRpc]
    void SetButtonsActive()
    {
        successButton.SetActive(true);
        failButton.SetActive(true);
    }

    [Command]
    void OnClicked(bool success)
    {
        votedForSuccess = success;
        votedForFail = !success;
        successButton.SetActive(false);
        failButton.SetActive(false);
    }

    [Client]
    public void VotePopupClosed()
    {
        successButton.SetActive(false);
        failButton.SetActive(false);
    }

    [Server]
    public void AfterMissionComplete()
    {
        if (result == MissionResult.Success && votedForSuccess)
        {
            Owner.Favour.Value += favourGain;
        }
        if (result == MissionResult.Fail && votedForFail)
        {
            Owner.Favour.Value += favourGain;
        }
    }
}