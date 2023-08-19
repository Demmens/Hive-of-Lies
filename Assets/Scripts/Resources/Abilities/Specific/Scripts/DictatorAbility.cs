using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictatorAbility : RoleAbility
{
    [SerializeField] hivePlayerVariable teamLeader;
    [SerializeField] IntVariable teamLeaderVoteTotal;

    public void AfterVoteBegin()
    {
        //If the team leader is the owner of this role, guarantee that they can't be voted out of team leader
        if (teamLeader.Value == Owner) teamLeaderVoteTotal.Value += 1000;
    }
}