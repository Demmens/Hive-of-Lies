using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerButtonDropdownItem : NetworkBehaviour
{
    [HideInInspector] public HoLPlayer Owner;
    public event System.Action<HoLPlayer> OnItemClicked;
    public event System.Action OnDestroyed;
    [SyncVar(hook = nameof(SetText))] public string Text;

    [SerializeField] TMPro.TMP_Text text;

    [Client]
    public void Click()
    {
        ServerClick();
    }
    [Command(requiresAuthority = false)]
    void ServerClick()
    {
        OnItemClicked?.Invoke(Owner);
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
        OnDestroyed?.Invoke();
    }

    [ClientRpc]
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    [ClientRpc]
    void SetText(string oldVal, string newVal)
    {
        text.text = newVal;                                                                                                         
    }
}
