using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerList : NetworkBehaviour
{
    [SerializeField] GameObject playerList;
    [SerializeField] GameObject playerButton;

    List<ulong> playersLoaded;

    [Tooltip("All players that are currently in the game")]
    [SerializeField] HoLPlayerSet serverPlayersLoaded;
    void Start()
    {
        playersLoaded = new List<ulong>();
    }

    /// <summary>
    /// Called on the server when any player connects to the game
    /// </summary>
    /// <param name="conn"></param>
    public void ServerOnClientLoaded(NetworkConnection conn)
    {
        List<ulong> loaded = new List<ulong>();
        serverPlayersLoaded.Value.ForEach(ply => loaded.Add(ply.PlayerID));
        OnClientLoaded(loaded);
    }

    [ClientRpc]
    void OnClientLoaded(List<ulong> loadedPlayers)
    {
        for (int i = 0; i < loadedPlayers.Count; i++)
        {
            ulong id = loadedPlayers[i];
            if (playersLoaded.Contains(id)) continue;

            GameObject button = Instantiate(playerButton);
            button.transform.SetParent(playerList.transform);
            button.GetComponent<PlayerButton>().SteamID = new CSteamID(id);
            playersLoaded.Add(id);
        }
    }
}
