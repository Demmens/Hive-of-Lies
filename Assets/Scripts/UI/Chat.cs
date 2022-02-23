using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;

public class Chat : MonoBehaviour
{
    [SerializeField] TMP_InputField input;

    [SerializeField] TMP_Text chat;

    private void Start()
    {
        NetworkServer.RegisterHandler<SendChatMessageMsg>(ServerGetMessage);
        NetworkClient.RegisterHandler<SendChatMessageMsg>(ClientGetMessage);
    }

    /// <summary>
    /// Player sent a chat message
    /// </summary>
    public void SendMessage()
    {
        if (input.text == "") return;
        NetworkClient.Send(new SendChatMessageMsg
        {
            userName = SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID()),
            message = input.text
        });

        input.text = "";
        input.Select();
        input.ActivateInputField();
    }

    void ServerGetMessage(NetworkConnection conn, SendChatMessageMsg msg)
    {
        if (msg.message == "") return;
        NetworkServer.SendToAll(msg);
    }

    void ClientGetMessage(SendChatMessageMsg msg)
    {
        chat.text += $"\n{msg.userName}: {msg.message}";
    }
}

public struct SendChatMessageMsg : NetworkMessage
{
    public string message;
    public string userName;
}
