using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericButton : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] RectTransform pos;
    [SerializeField] TMPro.TMP_Text cost;
    [SerializeField] GameObject favourIcon;
    [SerializeField] UnityEngine.UI.Button button;

    [SerializeField] IntVariable favour;

    int favourCost;
    public event System.Action OnClicked;

    private void Start()
    {
        favour.AfterVariableChanged += (val) => { button.interactable = val >= favourCost; };
    }

    public void Clicked()
    {
        OnClicked?.Invoke();
    }

    public void SetText(string txt)
    {
        text.text = txt;
    }

    public void SetCost(int f)
    {
        favourCost = f;
        cost.text = f.ToString();
        favourIcon.SetActive(true);
    }

    public void SetPos(Vector3 vec)
    {
        pos.SetPositionAndRotation(vec, new Quaternion());
    }
}
