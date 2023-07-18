using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSliderFromVector2 : MonoBehaviour
{
    public Slider Slider;

    public void MoveSlider(Vector2 input)
    {
        float value = Slider.direction == Slider.Direction.BottomToTop || Slider.direction == Slider.Direction.TopToBottom ? input.y : input.x;
        Slider.SetValueWithoutNotify(value);
    }
}
