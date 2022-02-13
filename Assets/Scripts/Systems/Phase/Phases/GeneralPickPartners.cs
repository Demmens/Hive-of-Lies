using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the TeamLeader to choose players to join them on the mission.
/// </summary>
public class GeneralPickPartners : GamePhase
{
    public override EGamePhase Phase
    {
        get
        {
            return EGamePhase.GeneralPickPartners;
        }
    }
    /// <summary>
    /// Number of players that go on the mission with the TeamLeader for each player count
    /// </summary>
    readonly Dictionary<int, int> partnerPlayerCounts = new Dictionary<int, int>()
    {
        {0,1},
        {6,2},
        {9,3}
    };

    /// <summary>
    /// Number of players that go on the mission with the TeamLeader in this game.
    /// </summary>
    int numPartners;

    /// <summary>
    /// List of players the TeamLeader has selected so far
    /// </summary>
    List<Player> playersSelected;

    void Start()
    {
        //Find the appropriate number of players that need to go on each mission.
        for (int i = 0; i <= GameInfo.PlayerCount; i++)
        {
            if (partnerPlayerCounts.TryGetValue(i, out int num)) numPartners = num;
        }
    }

    public override void Begin()
    {
        playersSelected = new List<Player>();
    }

    /// <summary>
    /// Called when the TeamLeader chooses a player to join them
    /// </summary>
    /// <param name="ply">The player that the TeamLeader has selected</param>
    public void GeneralSelectedPlayer(Player ply)
    {
        playersSelected.Add(ply);
        if (playersSelected.Count == numPartners)
        {
            //Highlight lock in button
        }
    }

    /// <summary>
    /// Called when the TeamLeader decides to unselect a player.
    /// </summary>
    /// <param name="ply"></param>
    public void GeneralUnselectedPlayer(Player ply)
    {
        playersSelected.Remove(ply);
    }

    /// <summary>
    /// Called when the TeamLeader locks in their choices of players for the mission.
    /// </summary>
    public void LockInChoices()
    {
        End();
    }
}
