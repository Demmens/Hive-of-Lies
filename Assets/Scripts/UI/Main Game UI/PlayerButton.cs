using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerButton : NetworkBehaviour
{
    #region SERVER
    /// <summary>
    /// The player this button is associated with
    /// </summary>
    HoLPlayer player;

    [SerializeField] HoLPlayerDictionary playersByConnection;
    #endregion

    #region CLIENT
    /// <summary>
    /// The PlayerButtonDropdownItems that appear when you click this button
    /// </summary>
    public List<GameObject> listItems = new List<GameObject>();

    [SerializeField] TMPro.TMP_Text playerName;
    #endregion

    public event System.Action OnClicked;

    /// <summary>
    /// Called on the client when the button is clicked
    /// </summary>
    public void ClientClick()
    {
        OnClicked?.Invoke();
    }

    [Server]
    public void SetPlayer(HoLPlayer ply)
    {
        player = ply;
    }
}
