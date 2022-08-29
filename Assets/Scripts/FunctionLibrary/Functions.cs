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
        return GameInfo.singleton.Players.FirstOrDefault(x => x.Value == ply).Key;
    }

    /// <summary>
    /// Finds the corresponding player from their ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Player FindPlayerFromID(ulong id)
    {
        return GameInfo.singleton.Players.FirstOrDefault(x => x.Value.ID == id).Value;
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

    /// <summary>
    /// Send a network message to all alive clients.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="msg"></param>
    /// <param name="channelId"></param>
    public static void SendToAlive<T>(T msg, int channelId = Channels.Reliable) where T : struct, NetworkMessage
    {
        foreach (KeyValuePair<NetworkConnection, Player> pair in GameInfo.singleton.Players)
        {
            pair.Key.Send(msg, channelId);
        } 
    }
}