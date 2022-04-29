using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StartGameButton : MonoBehaviour
{
    private void Start()
    {
        if (!NetworkServer.active) gameObject.SetActive(false);
    }

    public void StartGame()
    {
        HoLNetworkManager manager = NetworkManager.singleton as HoLNetworkManager;
        manager.StartGame("Game");
    }
}
