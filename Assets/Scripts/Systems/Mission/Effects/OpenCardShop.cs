using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OpenCardShop : MissionEffectBehaviour
{
    [SerializeField] Transform shopContents;
    [SerializeField] GameObject popup;
    [SerializeField] GameObject cardDisplayPrefab;

    [SerializeField] HivePlayerSet alivePlayers;
    [SerializeField] HivePlayerDictionary playersByConnection;
    [SerializeField] IntVariable playerCount;
    [SerializeField] IntVariable favour;

    [SerializeField] List<Card> possibleCards;
    [SerializeField] int numCardsInShop;

    Dictionary<HivePlayer, List<Card>> playerShops = new();
    List<NetworkConnection> playersClosedShop = new();

    [Server]
    public override void OnEffectTriggered()
    { 
    }

    public override void OnStartClient()
    {
        RequestCards();
    }

    [Command(requiresAuthority = false)]
    void RequestCards(NetworkConnectionToClient conn = null)
    {
        foreach (HivePlayer ply in alivePlayers.Value)
        {
            List<Card> cardsToSend = PickCards();
            playerShops[ply] = cardsToSend;
            GiveClientCards(ply.connectionToClient, cardsToSend);
        }
    }

    [TargetRpc]
    void GiveClientCards(NetworkConnection conn, List<Card> cards)
    {
        Debug.Log($"Client received {cards.Count} cards");
        foreach (Card card in cards)
        {
            CardDisplay cardDisplay = Instantiate(cardDisplayPrefab).GetComponent<CardDisplay>();
            cardDisplay.transform.SetParent(shopContents);
            cardDisplay.SetCard(card);
            cardDisplay.ShowCost(true);
            cardDisplay.OnClick += (card) =>
            {
                if (favour.Value < card.BuyValue) return;
                BuyCard(card);
                Destroy(cardDisplay.gameObject);
            };
        }
    }

    [Command(requiresAuthority = false)]
    public void BuyCard(Card card, NetworkConnectionToClient conn = null)
    {
        if (!playersByConnection.Value.TryGetValue(conn, out HivePlayer ply)) return;
        if (ply.Favour.Value < card.BuyValue) return;

        ply.Favour.Value -= card.BuyValue;
        ply.Deck.Value.DrawPile.Add(card);
        ply.Deck.Value.DrawPile.Shuffle();
    }

    [Client]
    public void ShopClosed()
    {
        ShopClosedServer();
    }

    [Command(requiresAuthority = false)]
    void ShopClosedServer(NetworkConnectionToClient conn = null)
    {
        playersClosedShop.Add(conn);
        if (playersClosedShop.Count == playerCount.Value) EndEffect();
    }

    List<Card> PickCards()
    {
        List<Card> picked = new();

        for (int i = 0; i < numCardsInShop; i++)
        {
            Card card = possibleCards.GetRandom();
            picked.Add(card);
        }

        return picked;
    }
}
