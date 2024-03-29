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
    [SerializeField] float secondsToFill;

    [SerializeField] Image image;

    bool isFilling;

    [Server]
    void Start()
    {
        var.AfterVariableChanged += val => OnVariableChange(val, useVariableForMax ? varMax : intMax);
        varMax.AfterVariableChanged += val => OnVariableChange(var, useVariableForMax ? val : intMax);
    }

    [ClientRpc]
    void OnVariableChange(int value, int max)
    {
        float maxFill = (float) value / max;
        StartCoroutine(Fill(maxFill));
    }

    IEnumerator Fill(float maxValue)
    {
        //If it's currently filling, wait for it to finish
        while (isFilling)
        {
            yield return null;
        }
        float initialValue = image.fillAmount;

        float time = 0;
        isFilling = true;
        
        while (time < secondsToFill)
        {
            time += Time.deltaTime;
            float t = time / secondsToFill;
            image.fillAmount = initialValue + ((maxValue - initialValue) * Easing.GradualStartEnd(t));
            yield return null;
        }

        isFilling = false;
    }
}
