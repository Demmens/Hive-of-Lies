using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionEffectText : MonoBehaviour
{
    [SerializeField] TMP_Text requirement;
    [SerializeField] TMP_Text effect;

    public void SetText(Comparator comparator, int value, List<MissionEffect> effects)
    {
        requirement.text = "";
        switch (comparator)
        {
            case Comparator.GreaterThan:
                requirement.text += ">";
                break;
            case Comparator.EqualTo:
                requirement.text += "=";
                break;
            case Comparator.LessThan:
                requirement.text += "<";
                break;
        }
        requirement.text += value.ToString();

        effect.text = "";

        foreach (MissionEffect eff in effects)
        {
            effect.text += eff.Description;
            effect.text += "\n";
        }

        //If there are no mission effects
        if (effect.text == "")
        {
            effect.text = "No Effect";
        }
        //Otherwise we can remove the last line break
        else
        {
            effect.text = effect.text.TrimEnd('\n');
        }
    }
}
