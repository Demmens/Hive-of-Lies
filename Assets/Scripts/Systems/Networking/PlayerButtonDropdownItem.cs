using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonDropdownItem : MonoBehaviour
{
    [HideInInspector] public ulong playerClicked;
    public event System.Action<ulong> OnItemClicked;
    public void Click()
    {
        OnItemClicked?.Invoke(playerClicked);
    }
}
