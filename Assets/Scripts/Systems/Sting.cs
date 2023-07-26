using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Sting : NetworkBehaviour
{
    #region SERVER
    [SerializeField] IntVariable playerCount;
    [SerializeField] HoLPlayerSet alivePlayers;
    [SerializeField] HoLPlayerSet beePlayers;
    [SerializeField] HoLPlayerSet waspPlayers;
    [SerializeField] HoLPlayerSet playersOnMission;
    [SerializeField] HoLPlayerDictionary playersByConnection;
    [SerializeField] NetworkingEvent playerWins;
    [SerializeField] Transform stingReticleCanvas;

    [SerializeField] GameObject dropdownButton;
    List<PlayerButtonDropdownItem> stingButtons = new();
    #endregion
    #region CLIENT
    [SerializeField] BoolVariable isAlive;
    [SerializeField] IntVariable favour;

    [SerializeField] GameObject stingPopup;
    [SerializeField] TMPro.TMP_Text targetText;
    [SerializeField] GameObject stingButton;
    [SerializeField] UnityEngine.UI.Button button;

    [SerializeField] List<GameObject> DisabledUI;
    bool isStinging;
    #endregion
    #region SHARED
    [SerializeField] int stingCost = 10;
    [SerializeField] GameObject reticle;
    #endregion

    [Client]
    public override void OnStartClient()
    {
        favour.AfterVariableChanged += AfterFavourChanged;
    }

    [Client]
    void AfterFavourChanged(int favour)
    {
        if (button == null) return;
        button.interactable = favour >= stingCost && !isStinging;
    }

    [Server]
    public void AfterRolesChosen()
    {
        foreach (HoLPlayer ply in waspPlayers.Value)
        {
            ply.Target.AfterVariableChanged += (pl) => OnTargetChanged(ply, pl);
            ply.Target.Value = (beePlayers.Value.Count > 0) ? beePlayers.Value.GetRandom() : alivePlayers.Value.GetRandom();
        } 
    }

    [Server]
    void OnTargetChanged(HoLPlayer ply, HoLPlayer target)
    {
        //If their target is unset
        if (ply.Target.Value == null)
        {
            UnsetClientTarget(ply.connectionToClient);
            return;
        }

        RoleData role = ply.Target.Value.Role.Value.Data;
        SetClientTarget(ply.connectionToClient, role.RoleName, role.Description);
    }

    [Server]
    public void OnMissionStart()
    {
        foreach (HoLPlayer wasp in waspPlayers.Value)
        {
            if (playersOnMission.Value.Contains(wasp)) SetStingInteractable(wasp.connectionToClient, true);
        }
    }

    [Server]
    public void OnMissionEnd()
    {
        foreach (HoLPlayer wasp in waspPlayers.Value)
        {
            SetStingInteractable(wasp.connectionToClient, false);
        }
    }

    [TargetRpc]
    void SetStingInteractable(NetworkConnection conn, bool active)
    {
        if (favour < stingCost) return;
        stingButton.GetComponent<UnityEngine.UI.Button>().interactable = active;
    }

    [TargetRpc]
    void SetClientTarget(NetworkConnection conn, string targetName, string targetDescription)
    {
        stingButton.SetActive(true);
        stingButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        GameObject popup = Instantiate(stingPopup);
        popup.GetComponent<Notification>().SetText($"Your target is the {targetName}:\n{targetDescription}");
        targetText.text = targetName.ToUpper() + "\n" + targetDescription;
    }

    [TargetRpc]
    void UnsetClientTarget(NetworkConnection conn)
    {
        stingButton.SetActive(false);
    }

    [Client]
    public void ClickSting()
    {
        if (favour < stingCost) return;
        isStinging = true;
        stingButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        PlayerStingClicked();
    }

    [Command(requiresAuthority = false)]
    void PlayerStingClicked(NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        if (ply.Team == Team.Bee) return;
        if (!playersOnMission.Value.Contains(ply)) return;

        ply.Favour.Value -= stingCost;
        //GameObject ret = Instantiate(reticle);
        //NetworkServer.Spawn(ret, conn);

        OnPlayerSting();

        foreach (HoLPlayer pl in playersOnMission.Value)
        {
#if !UNITY_EDITOR
            //Can't sting yourself
            if (pl == ply) continue;
#endif
            PlayerButtonDropdownItem item = pl.Button.AddDropdownItem(dropdownButton, ply);
            item.OnItemClicked += (tgt) => StingTargetDecided(ply,tgt);
            item.OnItemClicked += (tgt) => Destroy(item);
            stingButtons.Add(item);
        }
    }

    [ClientRpc]
    public void OnPlayerSting()
    {
        foreach (GameObject obj in DisabledUI)
        {
            obj.SetActive(false);
        }
    }

    [Server]
    void StingTargetDecided(HoLPlayer stinger, HoLPlayer target)
    {
        foreach (PlayerButtonDropdownItem item in stingButtons) Destroy(item);

        if (stinger.Target.Value == target)
        {
            playerWins?.Invoke(stinger.connectionToClient);
            return;
        }

        alivePlayers.Remove(stinger);
        playersByConnection.Value.Remove(stinger.connectionToClient);
        stinger.IsAlive.Value = false;
        playerCount--;
        ClientStingIncorrect(stinger.connectionToClient);
    }

    [ClientRpc]
    public void OnStingIncorrect()
    {
        foreach (GameObject obj in DisabledUI)
        {
            obj.SetActive(true);
        }
    }

    [TargetRpc]
    void ClientStingIncorrect(NetworkConnection conn)
    {
        isAlive.Value = false;
    }
}
