using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChangeUIOnIntVariable : NetworkBehaviour
{
    [SerializeField] IntVariable var;
    [SerializeField] TMPro.TMP_Text text;

    public override void OnStartServer()
    {
        var.AfterVariableChanged += AfterVarChanged;
    }

    [ClientRpc]
    public void AfterVarChanged(int val)
    {
        text.text = val.ToString();
    }
}
