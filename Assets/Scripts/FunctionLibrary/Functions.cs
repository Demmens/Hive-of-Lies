using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public static class Functions
{
    /// <summary>
    /// Finds the corresponding networkconnection for a given player
    /// </summary>
    /// <param name="ply"></param>
    /// <returns></returns>
    public static NetworkConnection FindConnectionFromPlayer(Player ply)
    {
        return GameInfo.Players.FirstOrDefault(x => x.Value == ply).Key;
    }

    /// <summary>
    /// Whether a list of players contains a player of the given ID
    /// </summary>
    /// <param name="playerList"></param>
    /// <param name="playerID">The ID of the player to find</param>
    /// <returns></returns>
    public static bool Contains(this List<Player> playerList, ulong playerID) {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].ID == playerID) return true;
        }

        return false;
    }
}

