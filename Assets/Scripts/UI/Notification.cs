using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class Notification : MonoBehaviour
{
    /// <summary>
    /// The text of the notification
    /// </summary>
    private TMP_Text text;

    /// <summary>
    /// The only notification script in the scene 
    /// </summary>
    public static Notification Singleton;

    /// <summary>
    /// Invoked when a notification is created
    /// </summary>
    public event NotificationCreated OnNotificationCreated;
    public delegate void NotificationCreated(string text, NotificationType type, NetworkConnection conn);


    void Start()
    {
        text = gameObject.GetComponentInChildren<TMP_Text>();
        Singleton = this;
        NetworkServer.RegisterHandler<CreatedNotificationMsg>(ServerReceiveNotification);
        NetworkClient.RegisterHandler<CreatedNotificationMsg>(ClientReceiveNotification);
    }

    /// <summary>
    /// Creates a notification
    /// </summary>
    /// <param name="newText"></param>
    public void CreateNotification(string newText, NotificationType type = NotificationType.Generic)
    {
        CreatedNotificationMsg msg = new CreatedNotificationMsg() { text = newText, type = type };
        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(msg);
        }
        else
        {
            text.text = newText;
            gameObject.SetActive(true);
            NetworkClient.Send(msg);
        }
    }

    /// <summary>
    /// If the server sends a notification, client reads it here
    /// </summary>
    /// <param name="msg"></param>
    private void ClientReceiveNotification(CreatedNotificationMsg msg)
    {
        CreateNotification(msg.text, msg.type);
    }

    /// <summary>
    /// Called automatically on the server when a notification is sent out on the client.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void ServerReceiveNotification(NetworkConnection conn, CreatedNotificationMsg msg)
    {
        OnNotificationCreated?.Invoke(msg.text, msg.type, conn);
    }

    public enum NotificationType
    {
        Generic,
        Investigate,

    }

    public struct CreatedNotificationMsg : NetworkMessage
    {
        public string text;
        public NotificationType type;
    }
}
