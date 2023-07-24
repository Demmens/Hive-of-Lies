using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    [SerializeField] Color green;
    [SerializeField] Color yellow;
    [SerializeField] Color red;

    [SerializeField] Color darkTextColour;
    [SerializeField] Color creamTextColour;

    public event System.Action<Color,Color> colorClicked;

    public void ClickGreen()
    {
        colorClicked?.Invoke(green, creamTextColour);
    }

    public void ClickYellow()
    {
        colorClicked?.Invoke(yellow, darkTextColour);
    }

    public void ClickRed()
    {
        colorClicked?.Invoke(red, creamTextColour);
    }
}
