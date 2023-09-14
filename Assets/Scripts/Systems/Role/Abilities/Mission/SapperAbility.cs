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
    [SerializeField] GameObject dropdownButton;
    [SerializeField] HivePlayerSet alivePlayers;
    [SerializeField] Card bombCard;
    List<PlayerButtonDropdownItem> shuffleButtons = new();
    #endregion
    
    public void OnAllDecksCreated()
    {
        CreatePopup();

        foreach (HivePlayer ply in alivePlayers.Value)
        {
            PlayerButtonDropdownItem item = ply.Button.AddDropdownItem(dropdownButton, Owner);
            item.OnItemClicked += PlayerChosen;
            item.OnItemClicked += (ply) => Destroy(item);
            shuffleButtons.Add(item);
        }
    }

    [TargetRpc]
    void CreatePopup()
    {
        GameObject pop = Instantiate(popup);
        pop.GetComponent<Notification>().SetText("Choose a player. Shuffle a bomb into that players deck.");
    }

    [Server]
    void PlayerChosen(HivePlayer player)
    {
        if (player.Deck.Value != null) ShuffleBomb(player);
        foreach (PlayerButtonDropdownItem item in shuffleButtons)
        {
            Destroy(item);
        }
    }

    [Server]
    void ShuffleBomb(HivePlayer player)
    {
        Deck deck = player.Deck;
        deck.DrawPile.Add(bombCard);
        deck.Shuffle();

        DisplayBombMessage(player.connectionToClient);
        if (player != Owner) DisplayBombMessageToOwner(player.DisplayName);
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
}
