using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.Localization;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    hiveNetworkManager networkManager
    {
        get
        {
            return NetworkManager.singleton as hiveNetworkManager;
        }
    }

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEnter;
    protected Callback<LobbyMatchList_t> lobbiesReceived;

    private const string hostAddressKey = "HostAddress";

    /// <summary>
    /// Server only storage of lobbyID
    /// </summary>
    public static CSteamID LobbyID;

    /// <summary>
    /// How many players are currently in this steam lobby
    /// </summary>
    public static int LobbySize => SteamMatchmaking.GetNumLobbyMembers(LobbyID);

    ELobbyType LobbyType = ELobbyType.k_ELobbyTypePublic;

    [SerializeField] GameObject joiningBox;
    [SerializeField] TMPro.TMP_Text joiningText;
    [SerializeField] LocalizedString joiningGameString;
    [SerializeField] LocalizedString lobbyNameString;

    [SerializeField] Button joinButton;
    [SerializeField] GameObject lobbyListItemPrefab;
    [SerializeField] Transform lobbyList;
    LobbyListItem lastSelectedLobby;
    int lobbySize;
    [SerializeField] TMPro.TMP_Text lobbySizeText;

    [SerializeField] StringVariable gameVersion;
    [SerializeField] GameObject mismatchedVersionPopup;
    [SerializeField] TMPro.TMP_Text mismatchedVersionText;
    [SerializeField] LocalizedString clientOutdatedString;
    [SerializeField] LocalizedString serverOutdatedString;

    private void Start()
    {
        if (!SteamManager.Initialized) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEnter = Callback<LobbyEnter_t>.Create(LobbyEnter);
        lobbiesReceived = Callback<LobbyMatchList_t>.Create(OnLobbiesReceived);
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        networkManager.StartHost();

        CSteamID lobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(lobbyID, hostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(lobbyID, "name", string.Format(lobbyNameString.GetLocalizedString(), SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID())));
        SteamMatchmaking.SetLobbyData(lobbyID, "version", gameVersion.Value);
    }

    /// <summary>
    /// Called on the client when they request to join someone elses lobby
    /// </summary>
    /// <param name="callback"></param>
    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        joiningBox.SetActive(true);
        joiningText.text = string.Format(joiningGameString.GetLocalizedString(), SteamFriends.GetFriendPersonaName(callback.m_steamIDFriend));
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    /// <summary>
    /// Called on the client when their request to join the lobby is accepted
    /// </summary>
    /// <param name="callback"></param>
    void LobbyEnter(LobbyEnter_t callback)
    {
        Debug.Log($"Lobbysize = {SteamMatchmaking.GetNumLobbyMembers(new CSteamID(callback.m_ulSteamIDLobby))}");
        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        string lobbyVersion = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "version");

        //If we're on the correct version, we can just join
        if (lobbyVersion == gameVersion.Value)
        {
            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
            return;
        }

        string[] serverVersionTable = lobbyVersion.Split(".");
        string[] clientVersionTable = gameVersion.Value.Split(".");

        if (serverVersionTable.Length != clientVersionTable.Length) Debug.LogError("Versions have differing numbers of decimal points for some reason");

        Debug.Log(lobbyVersion);

        for (int i = 0; i < serverVersionTable.Length; i++)
        {
            int serverInt = int.Parse(serverVersionTable[i]);
            int clientInt = int.Parse(clientVersionTable[i]);

            if (serverInt == clientInt) continue;

            if (serverInt > clientInt) mismatchedVersionText.text = clientOutdatedString.GetLocalizedString();

            if (clientInt > serverInt) mismatchedVersionText.text = serverOutdatedString.GetLocalizedString();

            joiningBox.SetActive(false);
            mismatchedVersionPopup.SetActive(true);
            SteamMatchmaking.LeaveLobby(LobbyID);
            break;
        }
    }

    public void SetLobbyPublic()
    {
        LobbyType = ELobbyType.k_ELobbyTypePublic;
    }

    public void SetLobbyPrivate()
    {
        LobbyType = ELobbyType.k_ELobbyTypePrivate;
    }

    public void SetLobbyFriendsOnly()
    {
        LobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
    }

    public void SetLobbySize(float size)
    {
        lobbySize = Mathf.FloorToInt(size);
        lobbySizeText.text = size.ToString();
    }

    public void HostLobby()
    {
        networkManager.maxConnections = lobbySize;
        SteamMatchmaking.CreateLobby(LobbyType, networkManager.maxConnections);
        networkManager.ServerChangeScene(networkManager.LobbyScene);
    }

    public void RequestLobbies()
    {
        SteamMatchmaking.RequestLobbyList();
    }

    void OnLobbiesReceived(LobbyMatchList_t callback)
    {
        for (int i = 0; i < callback.m_nLobbiesMatching && i < 20; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            LobbyListItem item = Instantiate(lobbyListItemPrefab).GetComponent<LobbyListItem>();
            item.transform.SetParent(lobbyList);
            item.CurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
            item.MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);
            item.LobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "name");
            item.LobbyID = lobbyID;

            item.gameObject.GetComponent<Button>().onClick.AddListener(() => LobbySelected(item));
        }
    }

    void LobbySelected(LobbyListItem item)
    {
        if (lastSelectedLobby != null) 
        {
            lastSelectedLobby.Deselect();
        }
        lastSelectedLobby = item;
        joinButton.interactable = true;
    }

    public void LobbyListClosed()
    {
        joinButton.interactable = false;

        for (int i = 0; i < lobbyList.childCount; i++)
        {
            Destroy(lobbyList.GetChild(i).gameObject);
        }
    }

    public void JoinLobby()
    {
        SteamMatchmaking.JoinLobby(LobbyID);
    }
}