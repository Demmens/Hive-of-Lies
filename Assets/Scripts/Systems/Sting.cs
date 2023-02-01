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
    #endregion
    #region CLIENT
    [SerializeField] BoolVariable isAlive;
    [SerializeField] IntVariable favour;

    [SerializeField] GameObject stingPopup;
    [SerializeField] TMPro.TMP_Text targetText;
    [SerializeField] GameObject stingButton;
    [SerializeField] UnityEngine.UI.Button button;
    bool isStinging;
    #endregion
    #region SHARED
    [SerializeField] int stingCost = 10;
    #endregion

    [Client]
    public override void OnStartClient()
    {
        favour.AfterVariableChanged += AfterFavourChanged;
    }

    [Client]
    void AfterFavourChanged(int favour)
    {
        button.interactable = favour >= stingCost;
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
        GameObject popup = Instantiate(stingPopup);
        popup.GetComponent<Notification>().SetText($"Your target is the {targetName}:\n{targetDescription}");
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
        stingButton.SetActive(false);
        PlayerStingClicked();
    }

    [Command(requiresAuthority = false)]
    void PlayerStingClicked(NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer ply)) return;
        if (ply.Team == Team.Bee) return;

        ply.Favour.Value -= stingCost;
    }

    [Client]
    public void PlayerClicked(ulong ply)
    {
        if (!isStinging) return;

        StingTargetDecided(ply);
        isStinging = false;
    }

    [Command(requiresAuthority = false)]
    void StingTargetDecided(ulong id, NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HoLPlayer stinger)) return;

        for (int i = 0; i < alivePlayers.Value.Count; i++)
        {
            HoLPlayer ply = alivePlayers.Value[i];
            if (ply.PlayerID != id) continue;

            if (stinger.Target.Value == ply) StingCorrect(stinger);
            else StingIncorrect(stinger);

            break;
        }
    }

    [Server]
    void StingIncorrect(HoLPlayer ply)
    {
        alivePlayers.Remove(ply);
        playersByConnection.Value.Remove(ply.connectionToClient);
        ply.IsAlive.Value = false;
        playerCount--;
        ClientStingIncorrect(ply.connectionToClient);
    }

    [Server]
    void StingCorrect(HoLPlayer ply)
    {
        playerWins?.Invoke(ply.connectionToClient);
    }

    [TargetRpc]
    void ClientStingIncorrect(NetworkConnection conn)
    {
        isAlive.Value = false;
    }
}
