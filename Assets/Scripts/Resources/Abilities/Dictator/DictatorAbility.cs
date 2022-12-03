using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictatorAbility : RoleAbility
{
    [SerializeField] HoLPlayerVariable teamLeader;
    [SerializeField] IntVariable teamLeaderVoteTotal;
    public void OnAllPlayersVoted()
    {
        //If the team leader is the owner of this role, guarantee that they can't be voted out of team leader
        if (teamLeader == Owner) teamLeaderVoteTotal.Value += 1000;
    }
}