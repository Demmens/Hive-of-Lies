using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerButton : MonoBehaviour
{
    #region CLIENT
    /// <summary>
    /// The PlayerButtonDropdownItems that appear when you click this button
    /// </summary>
    public List<GameObject> listItems = new List<GameObject>();

    [SerializeField] TMPro.TMP_Text playerName;
    #endregion

    public event System.Action<ulong> OnClicked;

    public ulong playerID;

    /// <summary>
    /// Called on the client when the button is clicked
    /// </summary>
    public void Click()
    {
        OnClicked?.Invoke(playerID);
    }

    public void SetPlayer(string name, ulong id)
    {
        playerName.text = name;
        playerID = id;
    }
}
