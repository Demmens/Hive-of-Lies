using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyChat : Chat
{
    [Server]
    public void OnPlayerEnteredLobby(NetworkConnection conn)
    {
        if (!playersByNetworkConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;

        text += $"\n{ply.DisplayName} has joined the game";
    }
}
