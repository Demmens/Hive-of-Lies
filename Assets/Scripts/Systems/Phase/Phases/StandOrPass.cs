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

    #region Fields

    [SerializeField] GameInfo info;

    /// <summary>
    /// The amount of favour all players lose if nobody stands for the position of team leader.
    /// </summary>
    [SerializeField] int favourLostForNobodyStanding;

    /// <summary>
    /// Players that chose to stand for TeamLeader
    /// <para></para>
    /// Private counterpart to <see cref="StandingPlayers"/>
    /// </summary>
    List<Player> standingPlayers;

    /// <summary>
    /// Players that chose to pass.
    /// <para></para>
    /// Private counterpart to <see cref="PassedPlayers"/>
    /// </summary>
    List<Player> passedPlayers;

    /// <summary>
    /// List of players to boost the influence of when deciding TeamLeader.
    /// <para></para>
    /// Private counterpart to <see cref="PlayerBoosts"/>
    /// </summary>
    Dictionary<Player, int> playerBoosts;

    #endregion

    #region Properties

    public override EGamePhase Phase
    {
        get
        {
            return EGamePhase.DecideGeneral;
        }
    }

    /// <summary>
    /// Players that chose to stand for TeamLeader
    /// </summary>
    public List<Player> StandingPlayers
    {
        get
        {
            return standingPlayers;
        }
        set
        {
            standingPlayers = value;
        }
    }

    /// <summary>
    /// Players that chose to pass
    /// </summary>
    public List<Player> PassedPlayers
    {
        get
        {
            return passedPlayers;
        }
        set
        {
            passedPlayers = value;
        }
    }

    /// <summary>
    /// List of players to boost the influence of when deciding TeamLeader
    /// </summary>
    public Dictionary<Player, int> PlayerBoosts
    {
        get
        {
            return playerBoosts;
        }
        set
        {
            playerBoosts = value;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Delegate for <see cref="OnPlayerStandOrPass"/>
    /// </summary>
    /// <param name="ply">The player that made the decision</param>
    /// <param name="stood">True if they stood</param>
    public delegate void PlayerStandOrPass(Player ply, bool stood);
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
        standingPlayers = new List<Player>();
        passedPlayers = new List<Player>();
        playerBoosts = new Dictionary<Player, int>();

        NetworkServer.SendToAll(new StartStandOrPassMsg()
        {
            favourCost = GameInfo.CurrentMission.Data.FavourCost
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
        GameInfo.Players.TryGetValue(conn, out Player ply);

        //Make sure they haven't already voted
        if (passedPlayers.Contains(ply) || standingPlayers.Contains(ply)) return;

        if (msg.isStanding) standingPlayers.Add(ply);
        else passedPlayers.Add(ply);

        //Invoke event for a player deciding to stand or pass
        OnPlayerStandOrPass?.Invoke(ply, msg.isStanding);

        if (standingPlayers.Count + passedPlayers.Count == GameInfo.PlayerCount) ReceiveResults();
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
        if (standingPlayers.Count == 0)
        {
            foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.Players)
            {
                pair.Value.Favour -= favourLostForNobodyStanding;
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
        GameInfo.TeamLeader = standingPlayers[0];
        Debug.Log($"The team leader has been set to {GameInfo.TeamLeader.DisplayName}");

        //The Team Leader pays the favour cost of standing
        GameInfo.TeamLeader.Favour -= GameInfo.CurrentMission.Data.FavourCost;

        NetworkServer.SendToAll(new TeamLeaderChangedMsg()
        {
            ID = GameInfo.TeamLeader.SteamID,
            maxPartners = TeamLeaderPickPartners.NumPartners
        });

        End();
    }

    /// <summary>
    /// Sort the list of standing players
    /// </summary>
    void SortStandingList()
    {
        standingPlayers.Sort((a, b) =>
        {
            playerBoosts.TryGetValue(a, out int aBoost);
            playerBoosts.TryGetValue(b, out int bBoost);

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
    public CSteamID ID;
    public int maxPartners;
}