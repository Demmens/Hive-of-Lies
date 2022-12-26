using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    void Start()
    {
        dropdown.SetActive(false);
        serverPlayersLoaded.AfterItemAdded += ServerOnClientLoaded;
        //Call in case we're the last client to join
        //(server updates the list of connected players before we have a change to listen to the event)
        ServerOnClientLoaded();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CloseDropdown();
        }
    }

    public void OnMouseEnter() { isMouseOver = true; }
    public void OnMouseExit() { isMouseOver = false; }

    /// <summary>
    /// Called on the server when any player connects to the game
    /// </summary>
    /// <param name="conn"></param>
    [Server]
    void ServerOnClientLoaded(HoLPlayer pl = null)
    {
        List<ulong> loaded = new List<ulong>();
        serverPlayersLoaded.Value.ForEach(ply => loaded.Add(ply.PlayerID));
        OnClientLoaded(loaded);
    }

    [ClientRpc]
    void OnClientLoaded(List<ulong> loadedPlayers)
    {
        for (int i = 0; i < loadedPlayers.Count; i++)
        {
            ulong id = loadedPlayers[i];
            if (buttons.TryGetValue(id, out PlayerButton btn)) continue;

            GameObject button = Instantiate(playerButton);
            button.transform.SetParent(playerList.transform);
            PlayerButton plButton = button.GetComponent<PlayerButton>();
            plButton.ID = id;
            buttons.Add(id, plButton);
        }
    }

    /// <summary>
    /// Creates a dropdown item without adding it to any players
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
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
    public void CreateDropdown(ulong plyID)
    {
        if (!buttons.TryGetValue(plyID, out PlayerButton button)) return;

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

    void CloseDropdown()
    {
        activeItems.ForEach(item => item.gameObject.SetActive(false));
        activeItems = new();
        //Don't set to 0 bc in testing we will have a ulong id of 1 and this will break it.
        //We want this number to be an ID that NO PLAYER will have, ever.
        activePlayer = 1;

        dropdown.SetActive(false);
    }
}
