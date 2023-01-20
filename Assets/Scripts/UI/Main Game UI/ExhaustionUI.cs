using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ExhaustionUI : NetworkBehaviour
{
    HoLPlayer player;
    [SerializeField] TMPro.TMP_Text text;
    IntVariable exhaustion => player.Exhaustion;

    public void SetPlayer(HoLPlayer ply)
    {
        player = ply;
        exhaustion.AfterVariableChanged += OnExhaustionChanged;
    }

    [ClientRpc]
    void OnExhaustionChanged(int value) 
    {
        text.text = value.ToString();
    }


}
