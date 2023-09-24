using UnityEngine;
using UnityEngine.Events;

public class ChangeUIOnClientIntVariable : MonoBehaviour
{
    [SerializeField] IntVariable var;
    [SerializeField] UnityEvent<string> onChange;

    public void Start()
    {
        var.AfterVariableChanged += AfterVarChanged;
    }

    public void AfterVarChanged(int val)
    {
        onChange.Invoke(val.ToString());
    }
}
