using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerButton : NetworkBehaviour
{
    #region SERVER
    private HoLPlayer owner;
    public HoLPlayer Owner
    {
        get
        {
            return owner;
        }
        set
        {
            if (owner != null) owner.Exhaustion.AfterVariableChanged -= ChangeExhaustion;
            owner = value;
            ChangeOwner(value.DisplayName);
            value.Exhaustion.AfterVariableChanged += ChangeExhaustion;
        }
    }
    private List<PlayerButtonDropdownItem> activeItems = new();
    #endregion
    #region CLIENT
    [SerializeField] GameObject dropdown;
    [SerializeField] TMPro.TMP_Text playerNameText;
    [SerializeField] UnityEngine.UI.RawImage exhaustionUI;

    [SerializeField] Texture someExhaustion;
    [SerializeField] Texture heavyExhaustion;

    [SerializeField] Texture onMissionTexture;
    [SerializeField] Texture teamLeaderTexture;

    [SerializeField] GameObject isReady;
    [SerializeField] GameObject exhaustionObj;
    [SerializeField] GameObject onMissionObj;
    /// <summary>
    /// The ID of the player associated with this button
    /// </summary>
    [HideInInspector]
    public ulong ID;
    #endregion

    public override void OnStartClient()
    {
        Debug.Log("Should be starting the client");
        dropdown = Instantiate(dropdown).transform.GetChild(0).gameObject;
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

    /// <summary>
    /// Adds a new item to the dropdown. Simply destroy the item to remove it from the list
    /// </summary>
    /// <param name="ply">The player that will be able to view this item. Pass as null to allow all players to see it.</param>
    /// <param name="prefab">Prefab of the item to add</param>
    /// <returns>The instantiated item</returns>
    [Server]
    public PlayerButtonDropdownItem AddDropdownItem(GameObject prefab, HoLPlayer ply = null)
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
    public void ChangeExhaustion(int exhaustion)
    {
        if (exhaustion == 0) exhaustionObj.SetActive(false);
        else exhaustionObj.SetActive(true);

        if (exhaustion == 1) exhaustionUI.texture = someExhaustion;
        if (exhaustion == 2) exhaustionUI.texture = heavyExhaustion;
    }

    [ClientRpc]
    public void ChangeOnMission(bool isOnMission)
    {
        onMissionObj.SetActive(isOnMission);
        if (isOnMission) onMissionObj.GetComponent<UnityEngine.UI.RawImage>().texture = onMissionTexture;
    }

    [ClientRpc]
    public void ChangeTeamLeader(bool isTeamLeader)
    {
        onMissionObj.SetActive(isTeamLeader);
        if (isTeamLeader) onMissionObj.GetComponent<UnityEngine.UI.RawImage>().texture = teamLeaderTexture;
    }
}