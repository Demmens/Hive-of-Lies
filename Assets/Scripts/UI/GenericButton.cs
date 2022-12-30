using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericButton : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] RectTransform pos;
    public event System.Action OnClicked;

    public void Clicked()
    {
        OnClicked?.Invoke();
    }

    public void SetText(string txt)
    {
        text.text = txt;
    }

    public void SetPos(Vector3 vec)
    {
        pos.SetPositionAndRotation(vec, new Quaternion());
    }
}
