using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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


    void Start()
    {
        text = gameObject.GetComponentInChildren<TMP_Text>();
        Singleton = this;
    }

    /// <summary>
    /// Call to change the 
    /// </summary>
    /// <param name="newText"></param>
    public void CreateNotification(string newText)
    {
        text.text = newText;
        gameObject.SetActive(true);
    }
}
