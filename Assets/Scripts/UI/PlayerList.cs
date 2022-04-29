using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerList : MonoBehaviour
{
    [SerializeField] GameObject playerList;
    [SerializeField] GameObject playerButton;

    List<ulong> playersLoaded;
    void Start()
    {
        playersLoaded = new List<ulong>();
        NetworkClient.RegisterHandler<PlayerReadyMsg>(OnClientLoaded);
    }

    void OnClientLoaded(PlayerReadyMsg msg)
    {
        for (int i = 0; i < msg.loadedPlayers.Count; i++)
        {
            ulong id = (ulong)msg.loadedPlayers[i];
            if (playersLoaded.Contains(id)) continue;

            GameObject button = Instantiate(playerButton);
            button.transform.SetParent(playerList.transform);
            button.GetComponent<PlayerButton>().SteamID = new CSteamID(id);
            playersLoaded.Add(id);
        }
    }
}
