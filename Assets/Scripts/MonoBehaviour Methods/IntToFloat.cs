using UnityEngine;
using UnityEngine.Events;

public class IntToFloat : MonoBehaviour
{
    [SerializeField] UnityEvent<float> Output;
    public void Convert(int val)
    {
        Output.Invoke(val);
    }
}
