using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class CreateButton : RoleAbility
{
    [SerializeField] GameObject button;
    [SerializeField] bool hasCost;
    [SerializeField] int cost;
    [SerializeField] string text;

    [SerializeField] UnityEvent ClientClicked;
    [SerializeField] UnityEvent ServerClicked;

    bool created;

    [TargetRpc]
    public void Create()
    {
        Debug.Log("Trying to create the button");
        button = Instantiate(button);
        GenericButton btn = button.GetComponent<GenericButton>();
        if (hasCost) btn.SetCost(cost);
        btn.OnClicked += ClientClicked.Invoke;
        btn.OnClicked += ClickedOnServer;
        btn.SetText(text);
        btn.SetPos(new Vector3(1630, 95, 0));
        created = true;
    }

    [Command]
    void ClickedOnServer()
    {
        ServerClicked.Invoke();
        Owner.Favour.Value -= cost;
    }

    public void SetButtonActive(bool active)
    {
        if (active && !created) Create();
        button.SetActive(active);
    }
}
