using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class VariableFill : NetworkBehaviour
{
    [SerializeField] IntVariable var;

    [SerializeField] bool useVariableForMax;
    [SerializeField] IntVariable varMax;
    [SerializeField] int intMax;

    [SerializeField] Image image;


    void Start()
    {
        var.AfterVariableChanged += val => OnVariableChange();
        varMax.AfterVariableChanged += val => OnVariableChange();
    }

    [ClientRpc]
    void OnVariableChange()
    {
        image.fillAmount = (float)var.Value / (useVariableForMax ? varMax.Value : intMax);
    }
}
