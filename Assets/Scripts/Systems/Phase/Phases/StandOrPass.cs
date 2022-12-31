using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

/// <summary>
/// All players have the choice to either stand for TeamLeader, or pass. Those who stand must have enough favour.
/// <para></para>
/// The player with the most favour becomes the TeamLeader.
/// </summary>
public class StandOrPass : GamePhase
{
    [Tooltip("The playercount")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The number of partners on the mission")]
    [SerializeField] IntVariable numPartners;

    [Tooltip("The Team Leader")]
    [SerializeField] HoLPlayerVariable teamLeader;

    [Tooltip("The Current Mission")]
    [SerializeField] MissionVariable currentMission;

    [Tooltip("The players that chose to stand")]
    [SerializeField] HoLPlayerSet standingPlayers;

    [Tooltip("The players that chose to pass")]
    [SerializeField] HoLPlayerSet passedPlayers;

    [Tooltip("All players by their NetworkConnection")]
    [SerializeField] HoLPlayerDictionary players;

    [Tooltip("Invoked when this phase begins")]
    [SerializeField] GameEvent standOrPassBegin;

    /// <summary>
    /// List of players to boost the influence of when deciding TeamLeader
    /// </summary>
    public Dictionary<HoLPlayer, int> PlayerBoosts;

    #region Events
    [Tooltip("Invoked once all players have decided to stand or pass")]
    [SerializeField] GameEvent onAllPlayersStandOrPass;

    [Tooltip("Invoked once the team leader vote has been counted")]
    [SerializeField] GameEvent onTeamLeaderVoteCounted;

    [Tooltip("Invoked if nobody stands for the position of team leader.")]
    [SerializeField] GameEvent onNobodyStood;

    private int missionEffectsTriggered = 0;
    #endregion

    public override void Begin()
    {
        standingPlayers.Value = new();
        passedPlayers.Value = new();
        PlayerBoosts = new();

        standOrPassBegin?.Invoke();
    }

    /// <summary>
    /// Called when a player stands for team leader
    /// </summary>
    /// <param name="conn"></param>
    [Server]
    public void PlayerStood(NetworkConnection conn)
    {
        PlayerDecision(conn, true);
    }

    /// <summary>
    /// Called when a player passes the opportunity to stand for team leader
    /// </summary>
    /// <param name="conn"></param>
    [Server]
    public void PlayerPassed(NetworkConnection conn)
    {
        PlayerDecision(conn, false);
    }

    /// <summary>
    /// Called when a player decides to stand or pass
    /// </summary>
    /// <param name="ply">The player who made the decision</param>
    /// <param name="stood">True if they chose to stand</param>
    [Server]
    void PlayerDecision(NetworkConnection conn, bool isStanding)
    {
        Debug.Log("Player has stood or passed");
        if (!Active) return;
        if (!players.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        //Make sure they haven't already voted
        if (passedPlayers.Value.Contains(ply) || standingPlayers.Value.Contains(ply)) return;

        if (isStanding)
        {
            standingPlayers.Add(ply);
            //Lose favour when you stand
            ply.Favour.Value -= currentMission.Value.FavourCost;
        }
        else passedPlayers.Add(ply);

        if (standingPlayers.Value.Count + passedPlayers.Value.Count == playerCount) ReceiveResults();
    }

    /// <summary>
    /// Called once all players have decided to stand or pass
    /// </summary>
    [Server]
    void ReceiveResults()
    {
        Debug.Log("All players have stood or passed");
        //Invoke event for all players having made a decision
        onAllPlayersStandOrPass?.Invoke();

        
        if (standingPlayers.Value.Count == 0)
        {
            //If nobody stood for the position of team leader, and we don't currently have a team leader
            if (teamLeader.Value == null)
            {
                currentMission.Value.FailEffects.ForEach(effect =>
                {
                    effect.OnMissionEffectFinished += OnMissionEffectFinished;
                    effect.TriggerEffect();
                });
                return;
            }
        }
        else
        {
            //Now, since we've sorted, the player at the top of the list will be the Team Leader
            teamLeader.Value = standingPlayers.Value[0];
        }

        //Find the highest influence players who stood.
        SortStandingList();

        //Invoke event before determining the Team Leader
        onTeamLeaderVoteCounted?.Invoke();

        //If this has been subscribed to, there's a good chance the standings have changed, so we need to resort
        if (onTeamLeaderVoteCounted != null)
            SortStandingList();
        
        Debug.Log($"The team leader has been set to {teamLeader.Value.DisplayName}");

        //All unsuccessful players get their favour back
        standingPlayers.Value.ForEach(ply =>
        {
            if (ply != teamLeader.Value)
            {
                ply.Favour.Value += currentMission.Value.FavourCost;
            }
        });

        End();
    }

    /// <summary>
    /// Sort the list of standing players
    /// </summary>
    [Server]
    void SortStandingList()
    {
        standingPlayers.Value.Sort((a, b) =>
        {
            PlayerBoosts.TryGetValue(a, out int aBoost);
            PlayerBoosts.TryGetValue(b, out int bBoost);

            int result = (a.Favour + aBoost) - (b.Favour + bBoost);
            //If both players have the same amount of favour, we should randomly pick between them
            return result == 0 ? Random.Range(-1, 1) : result;
        });
    }

    void OnMissionEffectFinished()
    {
        if (++missionEffectsTriggered < currentMission.Value.FailEffects.Count) return;

        onNobodyStood?.Invoke();
    }
}