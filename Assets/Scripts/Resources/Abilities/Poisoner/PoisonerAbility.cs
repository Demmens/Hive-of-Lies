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
    [SerializeField] HoLPlayerVariable teamLeader;
    [SerializeField] IntVariable voteTotal;
    #endregion
    #region CLIENT
    [SerializeField] GameObject button;
    [SerializeField] IntVariable favour;
    #endregion

    public override void OnStartClient()
    {
        Instantiate(button);
        button.SetActive(false);
    }

    [Server]
    public void OnVoteResult()
    {
        //Only display the button if they actually got voted in
        if (voteTotal <= 0) return;
        SetButtonActive();
    }

    [Command(requiresAuthority = false)]
    public void OnPoison(NetworkConnectionToClient conn = null)
    {
        if (conn != Owner.Connection) return;
        if (cost > Owner.Favour.Value) return;

        Owner.Favour.Value -= cost;
        teamLeader.Value.Deck.Value.BeforeDraw += OnLeaderDraw;
    }

    [Server]
    public void OnLeaderDraw(ref Card card)
    {
        Deck deck = teamLeader.Value.Deck.Value;

        //Keep going until they have to pay for their draw
        if (teamLeader.Value.NextDrawCost > 0) deck.BeforeDraw -= OnLeaderDraw;

        //If the card is already bad, we're happy.
        if (card.Value <= 5) return;

        for (int i = 0; i < deck.DrawPile.Count; i++)
        {
            if (deck.DrawPile[i].Value <= 5) card = deck.DrawPile[i];
        }
    }

    [TargetRpc]
    void SetButtonActive()
    {
        button.SetActive(true);
        GenericButton btn = button.GetComponent<GenericButton>();
        btn.OnClicked += ClickedButton;
        btn.SetText($"{cost}f: Poison");
        btn.SetPos(new Vector3(Screen.width / 2, 20, 0));
    }

    [Client]
    void ClickedButton()
    {
        if (favour < cost) return;

        button.SetActive(false);
        OnPoison();
    }
}