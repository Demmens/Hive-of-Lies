using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerButton : MonoBehaviour
{
    /// <summary>
    /// The ID of the player associated with this button
    /// </summary>
    public ulong ID;

    public TMPro.TMP_Text PlayerName;

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
        onClicked?.Invoke(ID);
    }
}
