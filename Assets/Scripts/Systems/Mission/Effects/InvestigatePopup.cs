using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigatePopup : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;
    public void SetText(string txt)
    {
        text.text = txt;
    }
}
