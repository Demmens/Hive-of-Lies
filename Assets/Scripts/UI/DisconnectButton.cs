using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class DisconnectButton : MonoBehaviour
{
    [SerializeField] [Scene] string MenuScene;
    public void Click()
    {
        SteamMatchmaking.LeaveLobby(SteamLobby.LobbyID);
        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.StopServer();
        SceneManager.LoadScene(MenuScene);
    }
}
