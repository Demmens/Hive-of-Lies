using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCoords : MonoBehaviour
{
    [SerializeField] FloatVariable screenWidthCentre;
    [SerializeField] FloatVariable screenHeightCentre;

    [Tooltip("Reference to a transform that is located at the centre of the screen")]
    [SerializeField] Transform centralObject;

    [SerializeField] Transform roleButtonParent;
    public static Transform RoleButtonParent;

    void Start()
    {
        RoleButtonParent = roleButtonParent;
        screenWidthCentre.Value = centralObject.position.x;
        screenHeightCentre.Value = centralObject.position.y;
    }
}
