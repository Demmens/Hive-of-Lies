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
        NetworkManager.singleton.ServerChangeScene("Game");
    }
}
