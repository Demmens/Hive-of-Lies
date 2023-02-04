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
    [SerializeField] IntVariable freeDraws;
    bool isPoisoned;
    #endregion
    #region CLIENT
    [SerializeField] GameObject button;
    [SerializeField] IntVariable favour;
    #endregion

    public override void OnStartAuthority()
    {
        button = Instantiate(button);
        button.SetActive(false);
    }

    [Server]
    public void OnVoteResult()
    {
        //Only display the button if they actually got voted in
        if (voteTotal <= 0) return;
        SetButtonActive();
    }

    [Client]
    public void VotePopupClosed()
    {
        button.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    public void OnPoison(NetworkConnectionToClient conn = null)
    {
        if (conn != Owner.connectionToClient) return;
        if (cost > Owner.Favour.Value) return;

        Owner.Favour.Value -= cost;
        teamLeader.Value.Deck.Value.BeforeDraw += OnLeaderDraw;
        isPoisoned = true;
    }

    public void OnMissionEnd()
    {
        if (!isPoisoned) return;
        teamLeader.Value.Deck.Value.BeforeDraw -= OnLeaderDraw;
    }

    [Server]
    public void OnLeaderDraw(ref Card card)
    {
        Deck deck = teamLeader.Value.Deck.Value;

        //If the card is already bad, we're happy.
        if (card.Value <= 5) return;
        if (teamLeader.Value.NumDraws > freeDraws) return;

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
        btn.SetPos(new Vector3(Screen.width / 2, 80, 0));
    }

    [Client]
    void ClickedButton()
    {
        if (favour < cost) return;

        button.SetActive(false);
        OnPoison();
    }
}