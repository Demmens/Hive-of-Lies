using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionEffectText : MonoBehaviour
{
    [SerializeField] TMP_Text requirement;
    [SerializeField] TMP_Text effect;

    public void SetText(Comparator comparator, int value, MissionEffectTier effect)
    {
        requirement.text = "";
        switch (comparator)
        {
            case Comparator.GreaterThan:
                requirement.text += '\u2265';
                break;
            case Comparator.EqualTo:
                requirement.text += "=";
                break;
            case Comparator.LessThan:
                requirement.text += "<";
                break;
        }
        requirement.text += value.ToString();

        this.effect.text = "";

        foreach (MissionEffect eff in effect.effects)
        {
            if (eff.Description == "") continue;
            this.effect.text += eff.Description + "\n";
        }

        foreach (EMissionPlotPoint point in effect.plotPoints)
        {
            if (point.Description == "") continue;
            this.effect.text += point.Description + "\n";
        }

        //If there are no mission effects
        if (this.effect.text == "")
        {
            this.effect.text = "No Effect";
        }
        //Otherwise we can remove the last line break
        else
        {
            this.effect.text = this.effect.text.TrimEnd('\n');
        }
    }
}
