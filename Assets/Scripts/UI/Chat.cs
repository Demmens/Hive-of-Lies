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

    [SerializeField] protected HoLPlayerDictionary playersByNetworkConnection;

    [SyncVar(hook = nameof(OnTextChanged))] protected string text;

    /// <summary>
    /// Player sent a chat message
    /// </summary>
    [Client]
    public void SendMessage()
    {
        if (input.text == "") return;

        ServerGetMessage(input.text);

        input.text = "";
        input.Select();
        input.ActivateInputField();
    }

    [Command(requiresAuthority = false)]
    void ServerGetMessage(string message, NetworkConnectionToClient conn = null)
    {
        if (!playersByNetworkConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        if (message == "") return;

        ClientGetMessage(ply.DisplayName, message);
    }

    [ClientRpc]
    void ClientGetMessage(string name, string message)
    {
        message = KeywordManager.ModifyKeywordsInString(message);
        text += $"\n<b>{name}:</b> {message}";
    }

    [ClientRpc]
    void OnTextChanged(string oldVal, string newVal)
    {
        chat.text = newVal;
    }
}