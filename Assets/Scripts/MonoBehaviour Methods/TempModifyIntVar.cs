using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempModifyIntVar : MonoBehaviour
{
    [SerializeField] IntVariable variable;
    int modifier;

    public void Modify(int value)
    {
        variable.Value += value;
        modifier += value;
    }

    public void ResetVar()
    {
        variable.Value -= modifier;
        modifier = 0;
    }
}
