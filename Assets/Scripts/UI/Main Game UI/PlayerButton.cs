using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerButton : NetworkBehaviour
{
    #region SERVER
    private HivePlayer owner;
    public HivePlayer Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
            ChangeOwner(value.DisplayName);
        }
    }
    private List<PlayerButtonDropdownItem> activeItems = new();
    #endregion
    #region CLIENT
    [SerializeField] GameObject dropdown;
    [SerializeField] TMPro.TMP_Text playerNameText;

    [SerializeField] GameObject button;
    [SerializeField] GameObject trafficLights;
    [SerializeField] GameObject isReady;
    [SerializeField] GameObject exhaustionObj;
    [SerializeField] GameObject onMissionObj;
    [SerializeField] GameObject teamLeaderObj;
    [SerializeField] Image playerSprite;
    /// <summary>
    /// The ID of the player associated with this button
    /// </summary>
    [HideInInspector]
    public ulong ID;
    #endregion

    public override void OnStartClient()
    {
        dropdown = Instantiate(dropdown).transform.GetChild(0).gameObject;
        trafficLights = Instantiate(trafficLights);
        ClientAddDropdownItem(trafficLights);
        trafficLights.GetComponent<TrafficLights>().colorClicked += ChangeButtonColour;
    }

    [Client]
    public void Click()
    {
        dropdown.SetActive(true);
        dropdown.transform.SetPositionAndRotation(Mouse.current.position.ReadValue(), new Quaternion());
    }

    [ClientRpc]
    void ChangeOwner(string name)
    {
        playerNameText.text = name;
    }

    [Client]
    void ChangeButtonColour(Color colour, Color textColour)
    {
        Image img = button.GetComponent<Image>();

        //If the same colour was clicked twice, reset it.
        if (colour == img.color)
        {
            img.color = new Color(255 / 256f, 247 / 256f, 234 / 256f);
            playerNameText.color = new Color(101 / 256f, 64 / 256f, 43 / 256f);
        }
        else
        {
            img.color = colour;
            playerNameText.color = textColour;
        }
    }

    /// <summary>
    /// Adds a new item to the dropdown. Simply destroy the item to remove it from the list
    /// </summary>
    /// <param name="ply">The player that will be able to view this item. Pass as null to allow all players to see it.</param>
    /// <param name="prefab">Prefab of the item to add</param>
    /// <returns>The instantiated item</returns>
    [Server]
    public PlayerButtonDropdownItem AddDropdownItem(GameObject prefab, HivePlayer ply = null)
    {
        GameObject obj = Instantiate(prefab);
        NetworkServer.Spawn(obj);
        obj.SetActive(false);

        if (ply == null) RpcAddDropdownItem(obj);
        else TargetAddDropdownItem(ply.connectionToClient, obj);

        PlayerButtonDropdownItem item = obj.GetComponent<PlayerButtonDropdownItem>();
        activeItems.Add(item);
        item.OnDestroyed += () => activeItems.Remove(item);
        item.Owner = Owner;
        return item;
    }

    [ClientRpc]
    void RpcAddDropdownItem(GameObject obj)
    {
        ClientAddDropdownItem(obj);
    }

    [TargetRpc]
    void TargetAddDropdownItem(NetworkConnection conn, GameObject obj)
    {
        ClientAddDropdownItem(obj);
    }

    void ClientAddDropdownItem(GameObject obj)
    {
        obj.transform.SetParent(dropdown.transform);
        obj.SetActive(true);
    }

    public void SetReady(bool ready)
    {
        isReady.SetActive(ready);
    }

    [ClientRpc]
    public void ChangeOnMission(bool isOnMission)
    {
        onMissionObj.SetActive(isOnMission);
    }

    [ClientRpc]
    public void ChangeTeamLeader(bool isTeamLeader)
    {
        teamLeaderObj.SetActive(isTeamLeader);
    }

    [TargetRpc]
    public void ChangeSprite(NetworkConnection conn, Sprite sprite)
    {
        playerSprite.sprite = sprite;
    }
}