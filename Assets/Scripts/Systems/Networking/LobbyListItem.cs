using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;

public class LobbyListItem : MonoBehaviour
{
    string lobbyName;
    string gameMode;
    int maxPlayers;
    int currentPlayers;

    public CSteamID LobbyID;

    public string LobbyName 
    { 
        get 
        {
            return lobbyName;
        }
        set
        {
            lobbyName = value;
            nameText.text = lobbyName;
        }
    }

    public string GameMode
    {
        get
        {
            return gameMode;
        }
        set
        {
            gameMode = value;
            gameModeText.text = gameMode;
        }
    }

    public int MaxPlayers
    {
        get
        {
            return maxPlayers;
        }
        set
        {
            maxPlayers = value;
            playersText.text = $"{currentPlayers}/{maxPlayers}";
        }
    }

    public int CurrentPlayers
    {
        get
        {
            return currentPlayers;
        }
        set
        {
            currentPlayers = value;
            playersText.text = $"{currentPlayers}/{maxPlayers}";
        }
    }

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text playersText;
    [SerializeField] TMP_Text gameModeText;
    [SerializeField] UnityEngine.UI.Image colour;

    public void Select()
    {
        SteamLobby.LobbyID = LobbyID;
        colour.color = new Color(1, 1, 1, 0.3f);
    }

    public void Deselect()
    {
        colour.color = new Color(0, 0, 0, 0.15f);
    }
}
