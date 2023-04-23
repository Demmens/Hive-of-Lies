using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SapperAbility : RoleAbility
{
    #region CLIENT
    [SerializeField] GameObject popup;
    [SerializeField] GameObject dropdownButton;
    PlayerButtonDropdown dropdown;
    #endregion
    #region SERVER
    [SerializeField] HoLPlayerSet alivePlayers;
    [SerializeField] Card bombCard;
    List<PlayerButtonDropdownItem> shuffleButtons = new();
    #endregion


    public override void OnStartAuthority()
    {
        GameObject pop = Instantiate(popup);
        pop.GetComponent<Notification>().SetText("Choose a player. Shuffle a bomb into that players deck.");
        foreach (HoLPlayer ply in alivePlayers.Value)
        {
            PlayerButtonDropdownItem item = ply.Button.AddDropdownItem(dropdownButton);
            item.OnItemClicked += PlayerChosen;
            item.OnItemClicked += (ply) => Destroy(item);
        }
        
    }

    [Command]
    void PlayerChosen(HoLPlayer player)
    {
        if (player.Deck.Value != null) ShuffleBomb(player);
        foreach (PlayerButtonDropdownItem item in shuffleButtons)
        {
            Destroy(item);
        }
    }

    [Server]
    void ShuffleBomb(HoLPlayer player)
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
