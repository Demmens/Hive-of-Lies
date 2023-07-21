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


    void Start()
    {
        var.AfterVariableChanged += val => OnVariableChange();
        varMax.AfterVariableChanged += val => OnVariableChange();
    }

    [ClientRpc]
    void OnVariableChange()
    {
        float max = (float)var.Value / (useVariableForMax ? varMax.Value : intMax);
        StartCoroutine(Fill(image.fillAmount, max));
    }

    IEnumerator Fill(float initialValue, float maxValue)
    {
        //If it's currently filling, wait for it to finish
        while (isFilling)
        {
            yield return null;
        }

        float time = 0;
        isFilling = true;
        
        while (time < secondsToFill)
        {
            time += Time.deltaTime;
            float t = time / secondsToFill;
            image.fillAmount = initialValue + ((maxValue - initialValue) * t);
            yield return null;
        }
    }
}
