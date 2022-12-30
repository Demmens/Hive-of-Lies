using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;

    private HoLNetworkManager manager;

    private HoLNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = HoLNetworkManager.singleton as HoLNetworkManager;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        //LobbyController.singleton.FindLocalPlayer();
        LobbyController.singleton.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        //Manager.GamePlayers.Add(this);
        LobbyController.singleton.UpdateLobbyName();
        //sLobbyController.singleton.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        //Manager.GamePlayers.Remove(this);
        //LobbyController.singleton.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }
        if (isClient)
        {
            //LobbyController.singleton.UpdatePlayerList();
        }
    }
}
