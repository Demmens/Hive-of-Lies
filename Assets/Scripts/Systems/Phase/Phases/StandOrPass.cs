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

    /// <summary>
    /// The amount of favour all players lose if nobody stands for the position of team leader.
    /// </summary>
    [SerializeField] int favourLostForNobodyStanding;

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

    [Tooltip("Invoked when a player decides to stand or pass")]
    [SerializeField] GameEvent onPlayerStandOrPass;

    [Tooltip("Invoked once all players have decided to stand or pass")]
    [SerializeField] GameEvent onAllPlayersStandOrPass;

    [Tooltip("Invoked once the team leader vote has been counted")]
    [SerializeField] GameEvent onTeamLeaderVoteCounted;

    [Tooltip("Invoked if nobody stands for the position of team leader.")]
    [SerializeField] GameEvent onNobodyStood;

    #endregion

    public override void Begin()
    {
        standingPlayers.Value = new List<HoLPlayer>();
        passedPlayers.Value = new List<HoLPlayer>();
        PlayerBoosts = new Dictionary<HoLPlayer, int>();

        standOrPassBegin?.Invoke();
    }

    /// <summary>
    /// Called when a player decides to stand or pass
    /// </summary>
    /// <param name="ply">The player who made the decision</param>
    /// <param name="stood">True if they chose to stand</param>
    [Command(requiresAuthority = false)]
    public void PlayerDecision(bool isStanding, NetworkConnectionToClient conn = null)
    {
        Debug.Log("Player has stood or passed");
        if (!Active) return;
        players.Value.TryGetValue(conn, out HoLPlayer ply);

        //Make sure they haven't already voted
        if (passedPlayers.Value.Contains(ply) || standingPlayers.Value.Contains(ply)) return;

        if (msg.isStanding) standingPlayers.Add(ply);
        else passedPlayers.Add(ply);

        //Invoke event for a player deciding to stand or pass
        onPlayerStandOrPass?.Invoke();

        if (standingPlayers.Value.Count + passedPlayers.Value.Count == playerCount) ReceiveResults();
    }

    /// <summary>
    /// Called once all players have decided to stand or pass
    /// </summary>
    void ReceiveResults()
    {
        Debug.Log("All players have stood or passed");
        //Invoke event for all players having made a decision
        onAllPlayersStandOrPass?.Invoke();

        //If nobody stood for the position of team leader
        if (standingPlayers.Value.Count == 0)
        {
            foreach (KeyValuePair<NetworkConnection, HoLPlayer> pair in players.Value)
            {
                pair.Value.Favour.Value -= favourLostForNobodyStanding;
            }
            onNobodyStood?.Invoke();
            return;
        }

        //Find the highest influence players who stood.
        SortStandingList();

        //Invoke event before determining the Team Leader
        onTeamLeaderVoteCounted?.Invoke();

        //If this has been subscribed to, there's a good chance the standings have changed, so we need to resort
        if (onTeamLeaderVoteCounted != null)
            SortStandingList();

        //Now, since we've sorted, the player at the top of the list will be the Team Leader
        teamLeader.Value = standingPlayers.Value[0];
        Debug.Log($"The team leader has been set to {teamLeader.Value.DisplayName}");

        //The Team Leader pays the favour cost of standing
        teamLeader.Value.Favour.Value -= currentMission.Value.FavourCost;

        NetworkServer.SendToAll(new TeamLeaderChangedMsg()
        {
            ID = teamLeader.Value.PlayerID,
            maxPartners = numPartners
        });

        End();
    }

    /// <summary>
    /// Sort the list of standing players
    /// </summary>
    void SortStandingList()
    {
        standingPlayers.Value.Sort((a, b) =>
        {
            PlayerBoosts.TryGetValue(a, out int aBoost);
            PlayerBoosts.TryGetValue(b, out int bBoost);

            int result = (a.Favour + aBoost) - (b.Favour + bBoost);
            return result == 0 ? Random.Range(-1, 1) : result;
        });
    }
}