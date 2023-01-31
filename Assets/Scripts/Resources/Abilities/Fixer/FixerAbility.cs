using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FixerAbility : RoleAbility
{
    #region SERVER
    [SerializeField] int incrementAmount;
    [SerializeField] HoLPlayerSet playersOnMission;
    #endregion
    #region CLIENT
    [SerializeField] GameObject buttonPrefab;
    private GameObject button;
    [SerializeField] IntVariable favour;
    [SerializeField] string buttonText;
    #endregion
    #region SHARED
    [SerializeField] int cost;
    #endregion

    [Server]
    public override void OnRoleGiven()
    {
        CreateButton();
        Owner.Deck.Value.AfterPlay += OnSubmittedCard;
    }

    [Server]
    public void OnMissionStart()
    {
        if (!playersOnMission.Value.Contains(Owner)) return;

        ClientMissionStart();
    }

    [TargetRpc]
    void ClientMissionStart()
    {
        button.SetActive(true);
    }

    [TargetRpc]
    public void OnSubmittedCard(Card card)
    {
        button.SetActive(false);
    }

    [TargetRpc]
    void CreateButton()
    {
        button = Instantiate(buttonPrefab);
        button.SetActive(false);
        GenericButton btn = button.GetComponent<GenericButton>();
        btn.OnClicked += ClickedButton;
        btn.SetText($"{cost}f: {buttonText}");
        btn.SetPos(new Vector3(2 * Screen.width / 3, 70, 0));
    }

    [Client]
    void ClickedButton()
    {
        if (favour < cost) return;
        IncrementCardValue();
    }

    [Command]
    void IncrementCardValue()
    {
        if (Owner.Favour < cost) return;
        if (Owner.Deck.Value.Hand.Count == 0) return;
        if (Owner.Deck.Value.Hand[0].TempValue == 20) return;
        Owner.Deck.Value.Hand[0].TempValue += incrementAmount;
        Owner.Favour.Value -= cost;
    }
}
