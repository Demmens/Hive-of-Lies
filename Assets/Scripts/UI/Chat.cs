using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;

public class Chat : NetworkBehaviour
{
    [SerializeField] TMP_InputField input;

    [SerializeField] TMP_Text chat;

    /// <summary>
    /// Player sent a chat message
    /// </summary>
    [Client]
    public void SendMessage()
    {
        if (input.text == "") return;

        ServerGetMessage(SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID()), input.text);

        input.text = "";
        input.Select();
        input.ActivateInputField();
    }

    [Command(requiresAuthority = false)]
    void ServerGetMessage(string name, string message)
    {
        if (message == "") return;
        ClientGetMessage(name, message);
    }

    [ClientRpc]
    void ClientGetMessage(string name, string message)
    {
        chat.text += $"\n{name}: {message}";
    }
}