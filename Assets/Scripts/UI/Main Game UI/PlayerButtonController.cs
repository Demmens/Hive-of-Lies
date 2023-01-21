using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerButtonController : NetworkBehaviour
{
    #region SERVER
    [SerializeField] HoLPlayerSet alivePlayers;
    #endregion

    #region CLIENT
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] Transform buttonParent;

    [SerializeField] GameObject dropdownObject;
    PlayerButtonDropdown dropdown { 
        get
        {
            if (dropdownObject.scene.name == null)
            {
                dropdownObject = Instantiate(dropdownObject);
            }
            return dropdownObject.GetComponent<PlayerButtonDropdown>();
        }
        set
        {

        }
    }

    Dictionary<ulong, PlayerButton> buttons = new();
    #endregion


    [Server]
    public void CreateButtons()
    {
        foreach (HoLPlayer ply in alivePlayers.Value)
        {
            CreateButton(ply.DisplayName, ply.PlayerID);
            ply.Exhaustion.AfterVariableChanged += change => OnExhaustionChanged(ply.PlayerID, change);
        }
    }

    [ClientRpc]
    void CreateButton(string playerName, ulong id) 
    {
        GameObject buttonObject = Instantiate(buttonPrefab);
        buttonObject.transform.SetParent(buttonParent);
        PlayerButton button = buttonObject.GetComponent<PlayerButton>();
        button.OnClicked += OnButtonClicked;
        button.SetPlayer(playerName, id);
        buttons.TryAdd(id, button);
    }

    [Client]
    void OnButtonClicked(ulong ply)
    {
        if (!buttons.TryGetValue(ply, out PlayerButton button)) return;

        dropdown.CreateDropdown(button);
    }

    [ClientRpc]
    void OnExhaustionChanged(ulong ply, int change)
    {
        if (!buttons.TryGetValue(ply, out PlayerButton button)) return;

        button.gameObject.GetComponent<ExhaustionUI>().OnExhaustionChanged(change);
    }

    [Server]
    void PlayerDied(HoLPlayer ply)
    {
        ClientPlayerDied(ply.PlayerID);
    }

    [ClientRpc]
    void ClientPlayerDied(ulong ply)
    {
        if (!buttons.TryGetValue(ply, out PlayerButton button)) return;

        buttons.Remove(ply);
        Destroy(button.gameObject);
    }

    [Client]
    public PlayerButtonDropdownItem AddItem(ulong ply, GameObject item)
    {
        if (!buttons.TryGetValue(ply, out PlayerButton button)) return default;

        return dropdown.AddItem(button, item);
    }

    /// <summary>
    /// Adds a new item to the dropdown of all players
    /// </summary>
    /// <param name="prefab"></param>
    [Client]
    public PlayerButtonDropdownItem AddAll(GameObject prefab)
    {
        PlayerButtonDropdownItem item = null;
        foreach (KeyValuePair<ulong, PlayerButton> pair in buttons)
        {
            item = AddItem(pair.Key, prefab);
        }
        return item;
    }

    [Client]
    public void RemoveItem(ulong ply, GameObject item)
    {
        if (!buttons.TryGetValue(ply, out PlayerButton button)) return;

        dropdown.RemoveItem(button, item);
    }

    [Client]
    public void RemoveAll(GameObject prefab)
    {
        foreach (KeyValuePair<ulong, PlayerButton> pair in buttons)
        {
            RemoveItem(pair.Key, prefab);
        }
    }

    [Client]
    public PlayerButtonDropdownItem CreateItem(GameObject prefab)
    {
        return dropdown.CreateItem(prefab);
    }
}
