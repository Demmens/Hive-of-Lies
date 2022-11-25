using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team Wins", menuName = "Missions/Effects/Team Win")]
public class TeamWins : MissionEffect
{
    [SerializeField] Team Team;

    GameEnd end;
    public override EffectType Type => Team == Team.Wasp ?  EffectType.Negative : EffectType.Positive;

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
