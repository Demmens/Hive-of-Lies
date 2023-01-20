using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class PlayerButtonDropdown : NetworkBehaviour
{
    #region Game Objects
    [SerializeField] public GameObject dropdown;
    #endregion

    /// <summary>
    /// Key = Prefab, Value = Instantiated Object
    /// </summary>
    Dictionary<GameObject, PlayerButtonDropdownItem> itemsInDropdown = new();
    bool isMouseOver;
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
    public void MouseEnter() {
        Debug.Log("Enter");
        isMouseOver = true;
    }
    [Client]
    public void MouseExit() {
        Debug.Log("Exit");
        isMouseOver = false; 
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
    public PlayerButtonDropdownItem AddItem(PlayerButton btn, GameObject prefab)
    {
        //If this is the first time we're ever adding this dropdown item to any player
        if (!itemsInDropdown.TryGetValue(prefab, out PlayerButtonDropdownItem spawned)) spawned = CreateItem(prefab);

        //Check if the player already has that item
        if (btn.listItems.Contains(prefab))
        {
            Debug.LogWarning($"Player button {btn.playerID} already has dropdown item {prefab}");
            return spawned;
        }

        //Give the player the desired item
        btn.listItems.Add(prefab);

        //If the active player is the player we're adding the item to, we should set it active now.
        if (activePlayer == btn.playerID) spawned.gameObject.SetActive(true);

        return spawned;
    }

    /// <summary>
    /// Removes an item from the dropdown
    /// </summary>
    /// <param name="prefab">The prefab of the item to remove</param>
    [Client]
    public void RemoveItem(PlayerButton btn, GameObject prefab)
    {
        if (!itemsInDropdown.TryGetValue(prefab, out PlayerButtonDropdownItem spawned)) return;

        if (btn.listItems.Contains(prefab))
        {
            spawned.gameObject.SetActive(false);
            btn.listItems.Remove(prefab);
        }
    }


    /// <summary>
    /// Creates the dropdown when clicking on a player
    /// </summary>
    /// <param name="plyID">The player id of the clicked player</param>
    [Client]
    public void CreateDropdown(PlayerButton button)
    {
        //If the dropdown is currently open, close it so we can reset the dropdown items
        if (dropdown.activeInHierarchy) CloseDropdown();

        activePlayer = button.playerID;

        for (int i = 0; i < button.listItems.Count; i++)
        {
            if (!itemsInDropdown.TryGetValue(button.listItems[i], out PlayerButtonDropdownItem item))
            {
                Debug.LogError("Player button contains a prefab that isn't instantiated");
                return;
            }

            item.playerClicked = button.playerID;
            item.gameObject.SetActive(true);
            activeItems.Add(item);
        }

        dropdown.SetActive(true);
        dropdown.GetComponent<RectTransform>().position = Input.mousePosition;

        foreach (KeyValuePair<GameObject, PlayerButtonDropdownItem> pair in itemsInDropdown)
        {
            pair.Value.playerClicked = button.playerID;
        }
    }

    [Client]
    void CloseDropdown()
    {
        activeItems.ForEach(item => item.gameObject.SetActive(false));
        activeItems = new();

        activePlayer = 1;

        dropdown.SetActive(false);
    }
}
