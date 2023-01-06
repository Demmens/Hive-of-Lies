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
    [SerializeField] HoLPlayerDictionary playersByConnection;
    [SerializeField] HoLPlayerSet allPlayers;
    #endregion
    [Server]
    public void OnPlayerJoinedLobby(NetworkConnection conn)
    {
        allPlayers.Value.ForEach(ply =>
        {
            CreateClientPlayerItem(ply.DisplayName, ply.Connection.connectionId, ply.PlayerID);
        });
    }

    public void OnPlayerLeftLobby(NetworkConnection conn)
    {
        RemovePlayerItem(conn.connectionId);
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
}
