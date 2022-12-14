using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

/// <summary>
/// All players have the choice to either stand for TeamLeader, or pass. Those who stand must have enough influence.
/// <para></para>
/// The player with the most influence becomes the TeamLeader.
/// </summary>
public class StandOrPass : GamePhase
{

    /// <summary>
    /// The amount of favour all players lose if nobody stands for the position of team leader.
    /// </summary>
    [SerializeField] int favourLostForNobodyStanding;

    public override EGamePhase Phase => EGamePhase.DecideGeneral;

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

    /// <summary>
    /// List of players to boost the influence of when deciding TeamLeader
    /// </summary>
    public Dictionary<HoLPlayer, int> PlayerBoosts;

    #region Events

    /// <summary>
    /// Delegate for <see cref="OnPlayerStandOrPass"/>
    /// </summary>
    /// <param name="ply">The player that made the decision</param>
    /// <param name="stood">True if they stood</param>
    public delegate void PlayerStandOrPass(HoLPlayer ply, bool stood);
    /// <summary>
    /// Invoked when a player decides to stand or pass
    /// </summary>
    public event PlayerStandOrPass OnPlayerStandOrPass;

    /// <summary>
    /// Delegate for <see cref="OnAllPlayersStandOrPass"/>
    /// </summary>
    public delegate void AllPlayersStandOrPass();
    /// <summary>
    /// Invoked once all players have decided to stand or pass
    /// </summary>
    public event AllPlayersStandOrPass OnAllPlayersStandOrPass;

    /// <summary>
    /// Delegate for <see cref="OnTeamLeaderVoteCounted"/>
    /// </summary>
    public delegate void TeamLeaderVoteCounted();
    /// <summary>
    /// Invoked once the team leader vote has been counted
    /// </summary>
    public event TeamLeaderVoteCounted OnTeamLeaderVoteCounted;

    /// <summary>
    /// Delegate for <see cref="OnNobodyStood"/>
    /// </summary>
    public delegate void NobodyStood();
    /// <summary>
    /// Invoked if nobody stands for the position of team leader.
    /// </summary>
    public event NobodyStood OnNobodyStood;

    #endregion

    private void Start()
    {
        NetworkServer.RegisterHandler<PlayerStandOrPassMsg>(PlayerDecision);
    }

    public override void Begin()
    {
        standingPlayers.Value = new List<HoLPlayer>();
        passedPlayers.Value = new List<HoLPlayer>();
        PlayerBoosts = new Dictionary<HoLPlayer, int>();

        NetworkServer.SendToAll(new StartStandOrPassMsg()
        {
            favourCost = currentMission.Value.FavourCost
        });
    }

    /// <summary>
    /// Called when a player decides to stand or pass
    /// </summary>
    /// <param name="ply">The player who made the decision</param>
    /// <param name="stood">True if they chose to stand</param>
    public void PlayerDecision(NetworkConnection conn, PlayerStandOrPassMsg msg)
    {
        Debug.Log("Player has stood or passed");
        if (!Active) return;
        players.Value.TryGetValue(conn, out HoLPlayer ply);

        //Make sure they haven't already voted
        if (passedPlayers.Value.Contains(ply) || standingPlayers.Value.Contains(ply)) return;

        if (msg.isStanding) standingPlayers.Add(ply);
        else passedPlayers.Add(ply);

        //Invoke event for a player deciding to stand or pass
        OnPlayerStandOrPass?.Invoke(ply, msg.isStanding);

        if (standingPlayers.Value.Count + passedPlayers.Value.Count == playerCount) ReceiveResults();
    }

    /// <summary>
    /// Called once all players have decided to stand or pass
    /// </summary>
    void ReceiveResults()
    {
        Debug.Log("All players have stood or passed");
        //Invoke event for all players having made a decision
        OnAllPlayersStandOrPass?.Invoke();

        //If nobody stood for the position of team leader
        if (standingPlayers.Value.Count == 0)
        {
            foreach (KeyValuePair<NetworkConnection, HoLPlayer> pair in players.Value)
            {
                pair.Value.Favour.Value -= favourLostForNobodyStanding;
                pair.Key.Send(new SetFavourMsg()
                {
                    newFavour = pair.Value.Favour
                });
            }
            OnNobodyStood?.Invoke();
            return;
        }

        //Find the highest influence players who stood.
        SortStandingList();

        //Invoke event before determining the Team Leader
        OnTeamLeaderVoteCounted?.Invoke();

        //If this has been subscribed to, there's a good chance the standings have changed, so we need to resort
        if (OnTeamLeaderVoteCounted != null)
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

public struct PlayerStandOrPassMsg : NetworkMessage
{
    public bool isStanding;
}

public struct StartStandOrPassMsg : NetworkMessage
{
    public int favourCost;
}

public struct TeamLeaderChangedMsg : NetworkMessage
{
    public ulong ID;
    public int maxPartners;
}