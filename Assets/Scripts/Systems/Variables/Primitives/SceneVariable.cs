using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene", menuName = "Variable/Primitives/Scene")]
public class SceneVariable : Variable<string>
{
    [Tooltip("The initial value of this variable")]
    [Scene]
    private string initialValue;

    [Tooltip("The current value of this variable")]
    [Scene]
    private string currentValue;
}