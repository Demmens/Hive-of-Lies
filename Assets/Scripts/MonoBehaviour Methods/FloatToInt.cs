using UnityEngine;
using UnityEngine.Events;

public class FloatToInt : MonoBehaviour
{
    [SerializeField] UnityEvent<int> Output;
    public void Floor(float val)
    {
        Output.Invoke(Mathf.FloorToInt(val));
    }

    public void Ceil(float val)
    {
        Output.Invoke(Mathf.CeilToInt(val));
    }

    public void Round(float val)
    {
        Output.Invoke(Mathf.RoundToInt(val));
    }
}
