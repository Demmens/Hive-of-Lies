using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using System.Reflection;
using UnityEngine.Events;

public class HoLNetworkManager : NetworkManager
{
    [SerializeField] private HoLPlayer GamePlayerPrefab;

    [Tooltip("Alive players by their network connection")]
    [SerializeField] HoLPlayerDictionary alivePlayersByConnection;

    [Tooltip("All players by their network connection")]
    [SerializeField] HoLPlayerDictionary playersByConnection;

    [Tooltip("All alive players in the game")]
    [SerializeField] HoLPlayerSet alivePlayers;

    [Tooltip("All players in the game")]
    [SerializeField] HoLPlayerSet allPlayers;

    int playersLoaded = 0;

    [Tooltip("The number of players in the game")]
    [SerializeField] IntVariable playerCount;

    [Tooltip("The event to invoke when a player joins the server")]
    [SerializeField] NetworkingEvent playerJoinedServer;

    [Tooltip("The event to invoke when a player leaves the server")]
    [SerializeField] NetworkingEvent playerLeavesServer;

    [Tooltip("The event to invoke when a player loads into the game scene")]
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

            ply.PlayerID = (ulong) SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.LobbyID, playersByConnection.Value.Count);
            ply.DisplayName = SteamFriends.GetFriendPersonaName(new CSteamID(ply.PlayerID));
            ply.gameObject.name = "Player: " + ply.DisplayName;

            playersByConnection.Value[conn] = ply;
            alivePlayersByConnection.Value[conn] = ply;
            alivePlayers.Add(ply);
            allPlayers.Add(ply);

            DontDestroyOnLoad(ply.gameObject);

            NetworkServer.AddPlayerForConnection(conn, ply.gameObject);
            
            playerJoinedServer?.Invoke(conn);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        playerLeavesServer?.Invoke(conn);
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
                    alivePlayers.Value = new();
                    alivePlayers.Value.AddRange(allPlayers.Value);
                    alivePlayersByConnection.Value = new();
                    foreach(HoLPlayer ply in allPlayers.Value)
                    {
                        alivePlayersByConnection.Value[ply.connectionToClient] = ply;
                    }
                    allPlayersLoaded.Invoke();
                    //For if we want to reset the game
                    playersLoaded = 0;
                }));
            }
        }
        
    }

    public override void OnServerChangeScene(string scene)
    {
        if (scene != GameScene) return;

        allPlayers.Value.ForEach(ply => ply.ResetValues());

        Object[] variables = Resources.LoadAll("Variables");

        for (int i = 0; i < variables.Length; i++)
        {
            System.Type variableType = variables[i].GetType();
            FieldInfo info = null;

            if (typeof(Variable<>).IsAssignableFrom(variableType)) info = variableType.GetField(nameof(Variable<int>.Persistent), BindingFlags.Public | BindingFlags.Instance);
            else info = variableType.GetField(nameof(RuntimeSet<int>.Persistent), BindingFlags.Public | BindingFlags.Instance);

            bool isPersistent = (bool) info.GetValue(variables[i]);

            if (isPersistent) continue;

            variableType.GetMethod(nameof(Variable<int>.OnEnable)).Invoke(variables[i], new object[] { });
        }
    }
}