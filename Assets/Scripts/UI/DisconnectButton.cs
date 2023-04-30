using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class DisconnectButton : MonoBehaviour
{
    [SerializeField] SceneVariable MenuScene;
    public void Click()
    {
        SteamMatchmaking.LeaveLobby(SteamLobby.LobbyID);
        NetworkManager.singleton.StopServer();
        NetworkManager.singleton.StopClient();
        SceneManager.LoadScene(MenuScene.Value);
    }
}
