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

    public event NotificationCreated OnNotificationCreated;
    public delegate void NotificationCreated(string text, NotificationType type, NetworkConnection conn);


    void Start()
    {
        text = gameObject.GetComponentInChildren<TMP_Text>();
        Singleton = this;
        NetworkServer.RegisterHandler<CreatedNotificationsMsg>(ServerReceiveNotification);
    }

    /// <summary>
    /// Call to change the 
    /// </summary>
    /// <param name="newText"></param>
    public void CreateNotification(string newText, NotificationType type = NotificationType.Generic)
    {
        text.text = newText;
        gameObject.SetActive(true);

    }

    /// <summary>
    /// Called automatically on the server when a notification is sent out on the client.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void ServerReceiveNotification(NetworkConnection conn, CreatedNotificationsMsg msg)
    {
        OnNotificationCreated?.Invoke(msg.text, msg.type, conn);
    }

    public enum NotificationType
    {
        Generic,
        Investigate,

    }

    public struct CreatedNotificationsMsg : NetworkMessage
    {
        public string text;
        public NotificationType type;
    }
}
