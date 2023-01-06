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
    [SerializeField] TMP_Text text;

    /// <summary>
    /// Sets the notification text
    /// </summary>
    /// <param name="newText"></param>
    public void SetText(string newText)
    {
        text.text = newText;
    }

    /// <summary>
    /// Destroys the notification
    /// </summary>
    public void CloseNotification()
    {
        Destroy(gameObject);
    }
}
