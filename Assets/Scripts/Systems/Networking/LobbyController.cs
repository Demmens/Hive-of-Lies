using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Localization;

public class LobbyController : NetworkBehaviour
{
    #region CLIENT    
    public TMP_Text LobbyNameText;
    public GameObject LoadingText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;

    public ulong CurrentLobbyID;
    private Dictionary<int, PlayerListItem> playerListItems = new();
    #endregion
    #region SERVER
    [SerializeField] HivePlayerDictionary playersByConnection;
    [SerializeField] HivePlayerSet allPlayers;
    [SerializeField] GameModeVariable gameMode;

    [SerializeField] TMP_Text playersRequiredText;
    [SerializeField] LocalizedString playersRequiredString;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject inviteButton;
    #endregion

    [Tooltip("The version of the game we are running. Used to check clients are all using the same version")]
    [SerializeField] StringVariable gameVersion;

    private void Start()
    {
        CheckVersion(gameVersion.Value);
        if (NetworkServer.active)
        {
            inviteButton.SetActive(true);
            playersRequiredText.gameObject.SetActive(true);
            playersRequiredText.text = string.Format(playersRequiredString.GetLocalizedString(), gameMode.Value.MinPlayers);
        }

#if UNITY_EDITOR
        playersRequiredText.gameObject.SetActive(false);
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
        foreach (HivePlayer ply in allPlayers.Value)
        {
            CreateClientPlayerItem(ply.DisplayName, ply.connectionToClient.connectionId, ply.PlayerID);
        }
        UpdateLobbyName();

        if (allPlayers.Value.Count >= gameMode.Value.MinPlayers)
        {
            playersRequiredText.gameObject.SetActive(false);
            startGameButton.SetActive(true);
        }
    }

    [Server]
    public void OnPlayerLeftLobby(NetworkConnection conn)
    {
        RemovePlayerItem(conn.connectionId);

        if (allPlayers.Value.Count < gameMode.Value.MinPlayers)
        {
            playersRequiredText.gameObject.SetActive(true);
            startGameButton.SetActive(false);
        }
    }

    [ClientRpc]
    public void UpdateLobbyName()
    {
        CurrentLobbyID = (ulong) SteamLobby.LobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(SteamLobby.LobbyID, "name");
        LobbyNameText.gameObject.SetActive(true);
        LoadingText.SetActive(false);
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
        item.transform.SetSiblingIndex(PlayerListViewContent.transform.childCount-2);
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
        HiveNetworkManager manager = NetworkManager.singleton as HiveNetworkManager;
        manager.StartGame();
    }

    public void Invite()
    {
        SteamFriends.ActivateGameOverlayInviteDialog(SteamLobby.LobbyID);
    }
}
