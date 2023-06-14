using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Mirror;

public class PlayerButtonDropdown : MonoBehaviour
{
    bool isMouseOver;

    /// <summary>
    /// Really didn't want to have to use Update, but cannot figure out any other way. Might come back to this later
    /// </summary>
    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (isMouseOver) return;
        if (!gameObject.activeInHierarchy) return;

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(data, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject) return;
        }

        gameObject.SetActive(false);
    }
}
