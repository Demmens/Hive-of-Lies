using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using UnityEngine.Events;

public class HoLNetworkManager : NetworkManager
{
    [SerializeField] private HoLPlayer GamePlayerPrefab;

    [Tooltip("All players by their network connection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("All players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    int playersLoaded = 0;

    [Tooltip("The number of players in the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The event to invoke when a player loads into the game")]
    [SerializeField] NetworkingEvent playerLoaded;

    [Tooltip("The event to invoke when all players have loaded into the game")]
    [SerializeField] GameEvent allPlayersLoaded;

    [Tooltip("The scene we consider to be the game scene")]
    [Scene]
    public string GameScene;

    [Tooltip("The scene we consider to be the lobby scene")]
    [Scene]
    public string LobbyScene;

    /// <summary>
    /// Called when a player joins the server for the first time
    /// </summary>
    /// <param name="conn"></param>
    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == LobbyScene)
        {
            HoLPlayer ply = Instantiate(GamePlayerPrefab);
            ply.Connection = conn;

            ply.PlayerID = (ulong) SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.LobbyID, playersByConnection.Value.Count);
            ply.DisplayName = SteamFriends.GetFriendPersonaName(new CSteamID(ply.PlayerID));

            playersByConnection.Value[conn] = ply;
            allPlayers.Add(ply);

            DontDestroyOnLoad(ply.gameObject);

            NetworkServer.AddPlayerForConnection(conn, ply.gameObject);
        }
    }

    public void StartGame()
    {
        ServerChangeScene(GameScene);
        playerCount.Value = SteamMatchmaking.GetNumLobbyMembers(SteamLobby.LobbyID);
        Debug.Log($"Started game with {playerCount.Value} players");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        if (SceneManager.GetActiveScene().path == GameScene)
        {
            playerLoaded?.Invoke(conn);
            if (++playersLoaded == playerCount)
            {
                StartCoroutine(Coroutines.Delay(0.5f, () =>
                {
                    allPlayersLoaded.Invoke();
                    //For if we want to reset the game
                    playersLoaded = 0;
                }));
            }
        }
        
    }
}