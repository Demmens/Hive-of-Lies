using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerButton : MonoBehaviour
{
    /// <summary>
    /// Private counterpart to <see cref="ID"/>
    /// </summary>
    private ulong id;
    /// <summary>
    /// ID of the player associated with this button
    /// </summary>
    public ulong ID
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
            gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = SteamFriends.GetFriendPersonaName(new CSteamID(value));
        }
    }

    /// <summary>
    /// Whether this is selected or not
    /// </summary>
    public bool selected;

    /// <summary>
    /// The PlayerButtonDropdownItems that appear when you click this button
    /// </summary>
    public List<GameObject> listItems = new List<GameObject>();

    [Tooltip("Invoked when a button is clicked")]
    [SerializeField] UlongEvent onClicked;

    public void Click()
    {
        onClicked?.Invoke(id);
    }
}
