using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SapperAbility : RoleAbility
{
    #region CLIENT
    [SerializeField] GameObject popup;
    #endregion
    #region SERVER
    [SerializeField] HoLPlayerSet alivePlayers;
    [SerializeField] MissionResultVariable missionResult;
    [SerializeField] GameEvent allPlayersPlayed;
    [SerializeField] CardSet playedCards;
    HoLPlayer chosenPlayer;
    #endregion
    #region SHARED
    [SyncVar] bool playerChosen = false;
    #endregion

    public override void OnRoleGiven()
    {
        CreatePopup(Owner.Connection);
    }

    [TargetRpc]
    public void CreatePopup(NetworkConnection conn)
    {
        GameObject pop = Instantiate(popup);
        pop.GetComponent<Notification>().SetText("Choose a player. Shuffle a bomb into that players deck.");
    }

    [Client]
    public void PlayerChosen(ulong player)
    {
        if (playerChosen) return;
        
        ServerPlayerChosen(player);
    }

    [Command]
    void ServerPlayerChosen(ulong player)
    {
        if (playerChosen) return;
        playerChosen = true;

        foreach (HoLPlayer ply in alivePlayers.Value)
        {
            if (ply.PlayerID != player) continue;
            chosenPlayer = ply;
        }

        if (chosenPlayer.Deck.Value != null) ShuffleBomb();
    }

    [Server]
    void ShuffleBomb()
    {
        Card bomb = new Card(-100);
        Deck deck = chosenPlayer.Deck;
        bomb.DrawEffects.Add(OnBombDrawn);
        bomb.DestroyOnDraw = true;
        deck.DrawPile.Add(bomb);
        deck.Shuffle();

        DisplayBombMessage(chosenPlayer.Connection);
        if (chosenPlayer != Owner) DisplayBombMessageToOwner(chosenPlayer.DisplayName);
    }

    [TargetRpc]
    void DisplayBombMessage(NetworkConnection conn)
    {
        GameObject pop = Instantiate(popup);
        pop.GetComponent<Notification>().SetText("A bomb has been shuffled into your deck");
    }

    [TargetRpc]
    void DisplayBombMessageToOwner(string targetName)
    {
        GameObject pop = Instantiate(popup);
        pop.GetComponent<Notification>().SetText($"A bomb has been shuffled into {targetName}'s deck");
    }

    [Server]
    void OnBombDrawn()
    {
        missionResult.Value = MissionResult.Fail;
        playedCards.Value = new();
        allPlayersPlayed?.Invoke();
    }
}
