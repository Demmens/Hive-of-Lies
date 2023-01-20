using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ExhaustionUI : NetworkBehaviour
{

    [SerializeField] RawImage image;
    [SerializeField] Texture notExhausted;
    [SerializeField] Texture slightlyExhausted;
    [SerializeField] Texture veryExhausted;

    public void OnExhaustionChanged(int value) 
    {
        if (value == 0) image.texture = notExhausted;
        if (value == 1) image.texture = slightlyExhausted;
        if (value >= 2) image.texture = veryExhausted;
    }
}
