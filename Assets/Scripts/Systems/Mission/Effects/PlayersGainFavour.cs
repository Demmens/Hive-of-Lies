using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Players Gain Favour", menuName = "Missions/Effects/Specific/Players Gain Favour")]
public class PlayersGainFavour : MissionEffect
{
    [Tooltip("The players to give the favour to")]
    [SerializeField] HoLPlayerSet players;
    [Tooltip("How much favour to give them")]
    [SerializeField] int favourGain;

    public override void TriggerEffect()
    {
        foreach (HoLPlayer ply in players.Value)
        {
            ply.Favour.Value += favourGain;
            ply.Connection.Send(new ChangeFavourMsg() { favourIncrease = favourGain });
        }
        EndEffect();
    }
}
