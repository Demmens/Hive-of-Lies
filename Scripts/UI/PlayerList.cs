using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerList : MonoBehaviour
{
    [SerializeField] List<GameObject> PlayerButtons;
    void Start()
    {
        NetworkClient.RegisterHandler<PlayerReadyMsg>(OnClientLoaded);
    }

    void OnClientLoaded(PlayerReadyMsg msg)
    {
        for (int i = 0; i < msg.loadedPlayers.Count; i++)
        {
            PlayerButtons[i].GetComponentInChildren<PlayerButton>().SteamID = msg.loadedPlayers[i];
            PlayerButtons[i].SetActive(true);
        }
    }
}
