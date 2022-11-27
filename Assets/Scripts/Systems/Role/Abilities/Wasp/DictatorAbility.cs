using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dictator Ability", menuName = "Roles/Abilities/Wasp/Dictator")]
public class DictatorAbility : RoleAbility
{
    TeamLeaderVote teamLeaderVote;
    [SerializeField] BoolVariable isTeamLeader;
    void Start()
    {
        teamLeaderVote = FindObjectOfType<TeamLeaderVote>();

        //Listen for when the Team Leader vote is about to be decided
        teamLeaderVote.OnAllPlayersVoted.AddListener(AlwaysWinVote);
    }

    void AlwaysWinVote()
    {
        //If the team leader is the owner of this role, guarantee that they can't be voted out of team leader
        if (isTeamLeader) teamLeaderVote.VoteTotal += 1000;
    }
}