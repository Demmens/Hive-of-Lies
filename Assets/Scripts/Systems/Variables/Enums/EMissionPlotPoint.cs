using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plot Point", menuName = "Enums/Mission Plot Point")]
public class EMissionPlotPoint : ScriptableObject
{
    [SerializeField] string description;

    /// <summary>
    /// The description of this plot point
    /// </summary>
    public string Description { get { return description; } }
}
