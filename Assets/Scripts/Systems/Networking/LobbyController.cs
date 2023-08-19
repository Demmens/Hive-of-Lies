using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LobbyController : NetworkBehaviour
{
    #region CLIENT
    public TMP_Text LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;

    public ulong CurrentLobbyID;
    private Dictionary<int, PlayerListItem> playerListItems = new();
    #endregion
    #region SERVER
    [SerializeField] hivePlayerDictionary playersByConnection;
    [SerializeField] hivePlayerSet allPlayers;

    [SerializeField] GameObject playersRequiredText;
    [SerializeField] GameObject startGameButton;
    #endregion

    [Tooltip("The version of the game we are running. Used to check clients are all using the same version")]
    [SerializeField] StringVariable gameVersion;

    private void Start()
    {
        CheckVersion(gameVersion.Value);
        if (NetworkServer.active) playersRequiredText.SetActive(true);

#if UNITY_EDITOR
        playersRequiredText.SetActive(false);
        startGameButton.SetActive(true);
#endif
    }

    [Command(requiresAuthority = false)]
    void CheckVersion(string version, NetworkConnectionToClient conn = null)
    {
        //We're all good if the game version is the same
        if (gameVersion.Value == version) return;
        WrongClientVersion(conn);
    }

    [TargetRpc]
    void WrongClientVersion(NetworkConnection conn)
    {
        NetworkClient.Disconnect();
    }

    [Server]
    public void OnPlayerJoinedLobby(NetworkConnection conn)
    {
        foreach (hivePlayer ply in allPlayers.Value)
        {
            CreateClientPlayerItem(ply.DisplayName, ply.connectionToClient.connectionId, ply.PlayerID);
        }
        UpdateLobbyName();

        if (allPlayers.Value.Count >= 4)
        {
            playersRequiredText.SetActive(false);
            startGameButton.SetActive(true);
        }
    }

    [Server]
    public void OnPlayerLeftLobby(NetworkConnection conn)
    {
        RemovePlayerItem(conn.connectionId);

        if (allPlayers.Value.Count < 4)
        {
            playersRequiredText.SetActive(true);
            startGameButton.SetActive(false);
        }
    }

    [ClientRpc]
    public void UpdateLobbyName()
    {
        CurrentLobbyID = (ulong) SteamLobby.LobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(SteamLobby.LobbyID, "name");
    }

    [ClientRpc]
    public void CreateClientPlayerItem(string name, int connID, ulong steamID)
    {
        //If the item already exists, back out now
        if (playerListItems.TryGetValue(connID, out PlayerListItem _)) return;

        GameObject item = Instantiate(PlayerListItemPrefab);
        PlayerListItem itemScript = item.GetComponent<PlayerListItem>();

        itemScript.PlayerName = name;
        itemScript.ConnectionID = connID;
        itemScript.PlayerSteamID = steamID;
        itemScript.SetPlayerValues();

        item.transform.SetParent(PlayerListViewContent.transform);
        item.transform.localScale = Vector3.one;

        playerListItems.TryAdd(connID, itemScript);
    }

    [ClientRpc]
    public void RemovePlayerItem(int connID)
    {
        if (!playerListItems.TryGetValue(connID, out PlayerListItem item)) return;

        playerListItems[connID] = null;
        Destroy(item.gameObject);
    }

    public void StartGame()
    {
        hiveNetworkManager manager = NetworkManager.singleton as hiveNetworkManager;
        manager.StartGame();
    }
}
