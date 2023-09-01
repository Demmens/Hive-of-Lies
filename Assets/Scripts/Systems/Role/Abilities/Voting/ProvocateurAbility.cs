using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProvocateurAbility : RoleAbility
{
    [SerializeField] int favourGain = 5;
    [SerializeField] HivePlayerSet playersOnMission;
    [SerializeField] VoteSet votes;
    [SerializeField] IntVariable voteTotal;

    public void AllPlayersVoted()
    {
        if (votes.Value.Count(vote => vote.votes < 0) == 0) return;
        if (!playersOnMission.Value.Contains(Owner)) return;
        if (voteTotal <= 0) return;

        Owner.Favour.Value += favourGain;
    }
}
