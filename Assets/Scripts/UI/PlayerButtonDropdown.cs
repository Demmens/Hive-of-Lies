using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerButtonDropdown : MonoBehaviour
{
    public static PlayerButtonDropdown singleton;
    #region Game Objects
    [SerializeField] GameObject dropDown;
    #endregion

    /// <summary>
    /// Item = Prefab, Value = Instantiated Object
    /// </summary>
    Dictionary<GameObject, PlayerButtonDropdownItem> itemsInDropdown;
    ulong buttonClicked;
    bool isMouseOver;

    void Start()
    {
        singleton = this;
        itemsInDropdown = new Dictionary<GameObject, PlayerButtonDropdownItem>();
        dropDown.SetActive(false);
        ClientEventProvider.singleton.OnPlayerClicked += CreateDropdown;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dropDown.SetActive(false);
        }
    }

    public void OnMouseEnter() { isMouseOver = true; }
    public void OnMouseExit() { isMouseOver = false; }


    /// <summary>
    /// Adds a new item to the dropdown
    /// </summary>
    /// <param name="prefab">Prefab of the item to add</param>
    /// <returns>The instantiated item</returns>
    public PlayerButtonDropdownItem AddItem(GameObject prefab)
    {
        if (itemsInDropdown.TryGetValue(prefab, out PlayerButtonDropdownItem spawned))
        {
            spawned.gameObject.SetActive(true);
        }
        else
        {
            spawned = Instantiate(prefab, dropDown.GetComponent<Transform>()).GetComponent<PlayerButtonDropdownItem>();
            spawned.playerClicked = buttonClicked;
            itemsInDropdown.Add(prefab, spawned);
        }

        return spawned;
    }

    /// <summary>
    /// Adds new items to the dropdown
    /// </summary>
    /// <param name="prefab">The prefabs of the items to add to the list</param>
    public void AddItem(List<GameObject> prefabs)
    {
        foreach (GameObject prefab in prefabs)
        {
            AddItem(prefab);
        }
    }

    /// <summary>
    /// Removes an item from the dropdown
    /// </summary>
    /// <param name="prefab">The prefab of the item to remove</param>
    public void RemoveItem(GameObject prefab)
    {
        if (!itemsInDropdown.TryGetValue(prefab, out PlayerButtonDropdownItem spawned)) return;

        spawned.gameObject.SetActive(false);
    }

    /// <summary>
    /// Clear all the items from the dropdown
    /// </summary>
    public void ClearItems()
    {
        foreach (KeyValuePair<GameObject, PlayerButtonDropdownItem> pair in itemsInDropdown)
        {
            pair.Value.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Creates the dropdown when clicking on a player
    /// </summary>
    /// <param name="plyID">The player id of the clicked player</param>
    public void CreateDropdown(ulong plyID)
    {
        buttonClicked = plyID;
        dropDown.SetActive(true);
        dropDown.GetComponent<RectTransform>().position = Input.mousePosition;

        foreach (KeyValuePair<GameObject, PlayerButtonDropdownItem> pair in itemsInDropdown)
        {
            pair.Value.playerClicked = plyID;
        }
    }
}
