using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    HoLNetworkManager networkManager
    {
        get
        {
            return NetworkManager.singleton as HoLNetworkManager;
        }
    }

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEnter;

    private const string hostAddressKey = "HostAddress";

    /// <summary>
    /// Server only storage of lobbyID
    /// </summary>
    public static CSteamID LobbyID;
    /// <summary>
    /// The sole steam lobby instance
    /// </summary>
    public static SteamLobby singleton;
    /// <summary>
    /// How many players are currently in this steam lobby
    /// </summary>
    public static int LobbySize => SteamMatchmaking.GetNumLobbyMembers(LobbyID);

    [SerializeField] GameObject joiningBox;
    [SerializeField] TMPro.TMP_Text joiningText;

    private void Start()
    {
        if (!SteamManager.Initialized) { return; }
        //We only ever want to run this code once
        if (singleton != null) return;
        singleton = this;
        DontDestroyOnLoad(this);

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEnter = Callback<LobbyEnter_t>.Create(LobbyEnter);
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID()) + "'s lobby");
    }

    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        joiningBox.SetActive(true);
        joiningText.text = $"JOINING {SteamFriends.GetFriendPersonaName(callback.m_steamIDFriend).ToUpper()}'S GAME";
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    void LobbyEnter(LobbyEnter_t callback)
    {
        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
    }
}