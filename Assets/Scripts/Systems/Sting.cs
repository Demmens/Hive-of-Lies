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
    [SerializeField] HoLPlayerDictionary playersByConnection;
    [SerializeField] NetworkingEvent playerWins;
    [SerializeField] Transform stingReticleCanvas;

    [SerializeField] GameObject dropdownButton;
    List<PlayerButtonDropdownItem> stingButtons = new();
    #endregion
    #region CLIENT
    [SerializeField] BoolVariable isAlive;
    [SerializeField] BoolVariable onMission;
    [SerializeField] IntVariable favour;

    [SerializeField] GameObject stingPopup;
    [SerializeField] TMPro.TMP_Text targetText;
    [SerializeField] GameObject stingButton;
    [SerializeField] UnityEngine.UI.Button button;

    [SerializeField] List<GameObject> DisabledUI;
    [SerializeField] TMPro.TMP_Text stingActiveText;
    bool isStinging;
    bool stingLocked;
    #endregion
    #region SHARED
    [SerializeField] int stingCost = 10;
    [SerializeField] NetworkBehaviour reticle;
    #endregion

    [Client]
    public override void OnStartClient()
    {
        favour.AfterVariableChanged += (_) => UpdateButtonInteractable();
    }

    [Client]
    void UpdateButtonInteractable()
    {
        if (stingLocked)
        {
            button.interactable = false;
            return;
        }
        if (button == null) return;
        button.interactable = favour >= stingCost && !isStinging;
    }

    [ClientRpc]
    public void ToggleStingLocked()
    {
        stingLocked = !stingLocked;
        UpdateButtonInteractable();
    }

    [Server]
    public void AfterRolesChosen()
    {
        beePlayers.Value.Shuffle();
        for (int i = 0, j = 0; i < waspPlayers.Value.Count; i++)
        {
            HoLPlayer wasp = waspPlayers.Value[i];
            //If nobody else can possibly be a target, have no target I guess
            HoLPlayer target;
            if (beePlayers.Value.Count > 0)
            {
                //Loop back to duplicate targets if there are more wasps with stings than bees for whatever reason (some future gamemode maybe)
                target = beePlayers.Value[j % beePlayers.Value.Count];
            }
            else
            {
                //Not a good solution for wasps having wasps as targets, but for now this will never happen in an actual game, so I'm not figuring it out now.
                target = waspPlayers.Value[j % waspPlayers.Value.Count];
            }

            wasp.Target.Value = target;
            //If the wasp actually has a target set, move to the next bee so we don't get duplicates
            if (wasp.Target.Value != null) j++;

            //Do this now so we can do as much shenanigans with targets as we like before any popups appear
            OnTargetChanged(wasp, target);
            wasp.Target.AfterVariableChanged += (tgt) => OnTargetChanged(wasp, tgt);
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

    [TargetRpc]
    void SetClientTarget(NetworkConnection conn, string targetName, string targetDescription)
    {
        stingButton.SetActive(true);
        UpdateButtonInteractable();
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
        ToggleStingLocked();

        ply.Favour.Value -= stingCost;
        StingReticle retScript = reticle.GetComponent<StingReticle>();
        retScript.Owner = ply;
        retScript.SetActiveOnClients(true);
        retScript.ownerButtonPos = ply.Button.transform.position;
        reticle.netIdentity.AssignClientAuthority(ply.connectionToClient);

        OnPlayerSting(ply.Target.Value.Role.Value.Data.RoleName);

        foreach (HoLPlayer pl in alivePlayers.Value)
        {
#if !UNITY_EDITOR
            //Can't sting yourself
            if (pl == ply) continue;
#endif
            PlayerButtonDropdownItem item = pl.Button.AddDropdownItem(dropdownButton, ply);
            item.OnItemClicked += (tgt) => StingTargetDecided(ply,tgt);
            stingButtons.Add(item);
        }
    }

    [ClientRpc]
    public void OnPlayerSting(string targetRole)
    {
        stingActiveText.text = $"TARGET: {targetRole.ToUpper()}";
        stingActiveText.gameObject.SetActive(true);
        foreach (GameObject obj in DisabledUI)
        {
            obj.SetActive(false);
        }
    }

    [Server]
    void StingTargetDecided(HoLPlayer stinger, HoLPlayer target)
    {
        foreach (PlayerButtonDropdownItem item in stingButtons) Destroy(item);

        reticle.GetComponent<StingReticle>().SetActiveOnClients(false);
        reticle.netIdentity.RemoveClientAuthority();

        if (stinger.Target.Value == target)
        {
            playerWins?.Invoke(stinger.connectionToClient);
            return;
        }

        OnStingIncorrect();
        //Enable stinging again for all other wasps
        ToggleStingLocked();

        alivePlayers.Remove(stinger);
        playersByConnection.Value.Remove(stinger.connectionToClient);
        stinger.IsAlive.Value = false;
        playerCount--;
        Destroy(stinger.Button.gameObject);
        ClientStingIncorrect(stinger.connectionToClient);
    }

    [ClientRpc]
    public void OnStingIncorrect()
    {
        stingActiveText.gameObject.SetActive(false);
        //If the player has stung before, the UI should stay inactive
        if (isStinging) return;
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
