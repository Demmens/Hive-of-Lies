using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using UnityEngine.Events;

public class HoLNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    int playersLoaded = 0;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.LobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public void StartGame(string sceneName)
    {
        ServerChangeScene(sceneName);
        GameInfo.PlayerCount = SteamMatchmaking.GetNumLobbyMembers(SteamLobby.LobbyID);
        Debug.Log($"Started game with {GameInfo.PlayerCount} players");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (++playersLoaded == GameInfo.PlayerCount)
            {
                Debug.Log("All players loaded");
                StartCoroutine(Coroutines.Delay(0.5f, () =>
                {
                    NetworkServer.SendToAll(new TellAllPlayersReadyMsg() { });
                }));
            }
        }
        
    }

}

public struct TellAllPlayersReadyMsg : NetworkMessage
{

}