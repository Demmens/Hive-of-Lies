using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Easing
{
    public static float GradualStartEnd(float t)
    {
        t -= 0.5f;
        t *= Mathf.PI;
        t = Mathf.Sin(t) + 1;
        t /= 2;
        return t;
    }

    public static float GradualEnd(float t)
    {
        t *= Mathf.PI / 2;
        return Mathf.Sin(t);
    }

    public static float GradualStart(float t)
    {
        t += 1;
        t *= Mathf.PI / 2;
        return Mathf.Sin(t);
    }
}
