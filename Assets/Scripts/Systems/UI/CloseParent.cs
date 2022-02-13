using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseParent : MonoBehaviour
{
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
