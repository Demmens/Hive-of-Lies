using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour
{
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;
    public UnityEvent onClick;

    bool isMouseOver;

    // Really didn't want to have to use Update, but cannot figure out any other way. Might come back to this later
    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(data, results);

        bool mouseOver = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject) continue;
            mouseOver = true;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && mouseOver) onClick?.Invoke();

        if (mouseOver == isMouseOver) return;
        //If mouseOver changed this frame
        isMouseOver = mouseOver;

        if (isMouseOver) onMouseEnter?.Invoke();
        if (!isMouseOver) onMouseExit?.Invoke();
    }
}
