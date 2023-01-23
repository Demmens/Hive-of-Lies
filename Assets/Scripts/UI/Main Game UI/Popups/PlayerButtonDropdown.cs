using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class PlayerButtonDropdown : NetworkBehaviour
{
    #region Game Objects
    [SerializeField] GameObject dropdown;
    #endregion

    /// <summary>
    /// Key = Prefab, Value = Instantiated Object
    /// </summary>
    Dictionary<GameObject, PlayerButtonDropdownItem> itemsInDropdown = new();
    bool isMouseOver;

    [SerializeField] GameObject playerList;
    [SerializeField] GameObject playerButton;

    [Tooltip("All players that are currently in the game")]
    [SerializeField] HoLPlayerSet serverPlayersLoaded;

    private Dictionary<ulong, PlayerButton> buttons = new();
    private List<PlayerButtonDropdownItem> activeItems = new();
    private ulong activePlayer;

    /// <summary>
    /// Really didn't want to have to use Update, but cannot figure out any other way. Might come back to this later
    /// </summary>
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (isMouseOver) return;
        if (!dropdown.activeInHierarchy) return;
        CloseDropdown();
    }

    [Client]
    public override void OnStartServer()
    {
        serverPlayersLoaded.AfterItemRemoved += PlayerDied;
    }

    [Client]
    public override void OnStartClient()
    {
        dropdown.SetActive(false);
    }

    [Client]
    public void MouseEnter() {
        isMouseOver = true;
    }
    [Client]
    public void MouseExit() {
        isMouseOver = false; 
    }

    [Server]
    public void AfterSetup()
    {
        List<ulong> ids = new();
        List<string> names = new();
        serverPlayersLoaded.Value.ForEach(ply =>
        {
            ids.Add(ply.PlayerID);
            names.Add(ply.DisplayName);
        });
        CreateButtons(ids, names);
    }

    [ClientRpc]
    void CreateButtons(List<ulong> ids, List<string> names)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            ulong id = ids[i];
            if (buttons.TryGetValue(id, out PlayerButton btn)) continue;

            GameObject button = Instantiate(playerButton);
            button.transform.SetParent(playerList.transform);
            PlayerButton plButton = button.GetComponent<PlayerButton>();
            plButton.ID = id;
            plButton.PlayerName.text = names[i];
            buttons.Add(id, plButton);
        }
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

    /// <summary>
    /// Creates a dropdown item without adding it to any players
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    [Client]
    public PlayerButtonDropdownItem CreateItem(GameObject prefab)
    {
        //Make sure this item doesn't exist already
        if (itemsInDropdown.TryGetValue(prefab, out PlayerButtonDropdownItem spawned))
        {
            return spawned;
        }

        spawned = Instantiate(prefab, dropdown.GetComponent<Transform>()).GetComponent<PlayerButtonDropdownItem>();
        itemsInDropdown.Add(prefab, spawned);

        spawned.gameObject.SetActive(false);

        return spawned;
    }

    /// <summary>
    /// Adds a new item to the dropdown
    /// </summary>
    /// <param name="player">The player button to add this object to</param>
    /// <param name="prefab">Prefab of the item to add</param>
    /// <returns>The instantiated item</returns>
    [Client]
    public PlayerButtonDropdownItem AddItem(ulong player, GameObject prefab)
    {
        //If this is the first time we're ever adding this dropdown item to any player
        if (!itemsInDropdown.TryGetValue(prefab, out PlayerButtonDropdownItem spawned)) spawned = CreateItem(prefab);

        //Check that the player we're trying to add the item to exists
        if (!buttons.TryGetValue(player, out PlayerButton btn))
        {
            Debug.LogError($"Trying to add dropdown item to a player button that doesn't exist: {player}");
            return null;
        }

        //Check if the player already has that item
        if (btn.listItems.Contains(prefab))
        {
            Debug.LogWarning($"Player button {player} already has dropdown item {prefab}");
            return spawned;
        }

        //Give the player the desired item
        btn.listItems.Add(prefab);

        //If the active player is the player we're adding the item to, we should set it active now.
        if (activePlayer == player) spawned.gameObject.SetActive(true);

        return spawned;
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

    /// <summary>
    /// Removes an item from the dropdown
    /// </summary>
    /// <param name="prefab">The prefab of the item to remove</param>
    [Client]
    public void RemoveItem(ulong player, GameObject prefab)
    {
        if (!itemsInDropdown.TryGetValue(prefab, out PlayerButtonDropdownItem spawned)) return;

        if (!buttons.TryGetValue(player, out PlayerButton button)) return;

        if (button.listItems.Contains(prefab))
        {
            spawned.gameObject.SetActive(false);
            button.listItems.Remove(prefab);
        }
    }

    [Client]
    public void RemoveAll(GameObject prefab)
    {
        if (!itemsInDropdown.TryGetValue(prefab, out PlayerButtonDropdownItem spawned)) return;

        foreach (KeyValuePair<ulong, PlayerButton> pair in buttons)
        {
            RemoveItem(pair.Key, prefab);
        }
    }


    /// <summary>
    /// Creates the dropdown when clicking on a player
    /// </summary>
    /// <param name="plyID">The player id of the clicked player</param>
    [Client]
    public void CreateDropdown(ulong plyID)
    {
        if (!buttons.TryGetValue(plyID, out PlayerButton button)) return;

        //If the dropdown is currently open, close it so we can reset the dropdown items
        if (dropdown.activeInHierarchy) CloseDropdown();

        activePlayer = plyID;

        for (int i = 0; i < button.listItems.Count; i++)
        {
            if (!itemsInDropdown.TryGetValue(button.listItems[i], out PlayerButtonDropdownItem item))
            {
                Debug.LogError("Player button contains a prefab that isn't instantiated");
                return;
            }

            item.playerClicked = plyID;
            item.gameObject.SetActive(true);
            activeItems.Add(item);
        }

        dropdown.SetActive(true);
        dropdown.GetComponent<RectTransform>().position = Input.mousePosition;

        foreach (KeyValuePair<GameObject, PlayerButtonDropdownItem> pair in itemsInDropdown)
        {
            pair.Value.playerClicked = plyID;
        }
    }

    [Client]
    void CloseDropdown()
    {
        activeItems.ForEach(item => item.gameObject.SetActive(false));
        activeItems = new();
        //Don't set to 0 because the default id is 0.
        //We want this number to be an ID that NO PLAYER will have, ever.
        activePlayer = 1;

        dropdown.SetActive(false);
    }
}
