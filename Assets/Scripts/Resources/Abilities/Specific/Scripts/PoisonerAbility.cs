using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System.Linq;

public class PoisonerAbility : RoleAbility
{
    #region SERVER
    [SerializeField] int cost = 5;
    [SerializeField] int maxDraw = -1;
    [SerializeField] int numPoisonedDraws = 2;
    [SerializeField] hivePlayerVariable teamLeader;
    [SerializeField] IntVariable voteTotal;
    hivePlayer poisonedPlayer;
    #endregion
    #region CLIENT
    [SerializeField] GameObject button;
    [SerializeField] IntVariable favour;
    #endregion

    public override void OnStartAuthority()
    {
        button = Instantiate(button);
        button.SetActive(false);
        GenericButton btn = button.GetComponent<GenericButton>();
        btn.SetCost(cost);
        btn.OnClicked += ClickedButton;
        btn.SetText("Poison");
        button.transform.SetParent(ScreenCoords.RoleButtonParent);
    }

    public void OnVoteStart()
    {
        SetButtonActive();

        if (poisonedPlayer == null) return;

        poisonedPlayer.Deck.Value.BeforeDraw -= OnLeaderDraw;
        poisonedPlayer = null;
    }

    [Server]
    public void OnVoteResult()
    {
        //Refund the cost if the vote failed
        ClientVoteResult();
        if (voteTotal <= 0 && poisonedPlayer != null) Owner.Favour.Value += cost;
    }

    [TargetRpc]
    public void ClientVoteResult()
    {
        button.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    public void OnPoison(NetworkConnectionToClient conn = null)
    {
        if (conn != Owner.connectionToClient) return;
        if (cost > Owner.Favour.Value) return;

        Owner.Favour.Value -= cost;
        poisonedPlayer = teamLeader.Value;
        poisonedPlayer.Deck.Value.BeforeDraw += OnLeaderDraw;
    }

    [Server]
    public void OnLeaderDraw(ref Card card)
    {
        Deck deck = teamLeader.Value.Deck.Value;

        //If the card is already bad, we're happy.
        if (card.Value <= maxDraw) return;
        //If they've already drawn all their poisoned cards
        if (teamLeader.Value.NumDraws > numPoisonedDraws) return;

        for (int i = 0; i < deck.DrawPile.Count; i++)
        {
            if (deck.DrawPile[i].Value <= maxDraw) card = deck.DrawPile[i];
        }
    }

    [TargetRpc]
    void SetButtonActive()
    {
        button.SetActive(true);
    }

    [Client]
    void ClickedButton()
    {
        if (favour < cost) return;

        button.SetActive(false);
        OnPoison();
    }
}