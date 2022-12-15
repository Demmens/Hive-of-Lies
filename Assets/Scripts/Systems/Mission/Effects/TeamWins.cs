using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team Wins", menuName = "Missions/Effects/Specific/Team Win")]
public class TeamWins : MissionEffect
{
    [SerializeField] Team Team;

    GameEnd end;

    private void Start()
    {
        end = FindObjectOfType<GameEnd>();
    }
    public override void TriggerEffect()
    {
        end.EndGame(Team);
        EndEffect();
    }
}
