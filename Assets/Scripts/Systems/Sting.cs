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
        stingButton.GetComponentInChildren<UnityEngine.UI.Button>().interactable = favour >= stingCost;
    }

    [Server]
    public void AfterRolesChosen()
    {
        waspPlayers.Value.ForEach(ply =>
        {
            ply.Target.Value = beePlayers.Value.GetRandom();
            SetClientTarget(ply.Connection, ply.Role.Value.Data.RoleName);
        });    
    }

    [TargetRpc]
    void SetClientTarget(NetworkConnection conn, string targetName)
    {
        stingButton.SetActive(true);
        GameObject popup = Instantiate(stingPopup);
        popup.GetComponent<Notification>().SetText($"Your target is the {targetName}");
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

        foreach (HoLPlayer ply in alivePlayers.Value)
        {
            if (ply.PlayerID != id) continue;

            if (stinger.Target.Value == ply) StingCorrect(stinger);
            else StingIncorrect(stinger);
        }
    }

    [Server]
    void StingIncorrect(HoLPlayer ply)
    {
        alivePlayers.Remove(ply);
        playersByConnection.Value.Remove(ply.Connection);
        ply.IsAlive.Value = false;
        playerCount--;
        ClientStingIncorrect();
    }

    [Server]
    void StingCorrect(HoLPlayer ply)
    {
        playerWins?.Invoke(ply.Connection);
    }

    [ClientRpc]
    void ClientStingIncorrect()
    {
        isAlive.Value = false;
    }
}
