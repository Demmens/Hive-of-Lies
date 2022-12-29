using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Int", menuName = "Variable/Primitives/Int")]
public class IntVariable : Variable<int>
{
    public static IntVariable operator ++(IntVariable a)
    {
        a.Value++;
        return a;
    }
    public static IntVariable operator --(IntVariable a) {
        a.Value--;
        return a;
    }
}
