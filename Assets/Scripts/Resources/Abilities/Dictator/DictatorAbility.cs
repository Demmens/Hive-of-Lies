using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictatorAbility : RoleAbility
{
    TeamLeaderVote teamLeaderVote;
    void Start()
    {
        if (!Active) return;
        teamLeaderVote = FindObjectOfType<TeamLeaderVote>();

        //Listen for when the Team Leader vote is about to be decided
        teamLeaderVote.OnAllPlayersVoted.AddListener(AlwaysWinVote);
    }

    void AlwaysWinVote()
    {
        //If the team leader is the owner of this role, guarantee that they can't be voted out of team leader
        if (GameInfo.TeamLeaderID == Owner.SteamID)
            teamLeaderVote.VoteTotal += 1000;
    }
}