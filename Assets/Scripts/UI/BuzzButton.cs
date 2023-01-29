using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzButton : MonoBehaviour
{
    public ulong ID;

    public TMPro.TMP_Text PlayerName;

    public event System.Action<ulong> OnClicked;

    public void Click()
    {
        OnClicked?.Invoke(ID);
    }
}
