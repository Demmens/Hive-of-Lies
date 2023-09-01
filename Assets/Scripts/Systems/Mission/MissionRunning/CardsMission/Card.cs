using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Create card")]
public class Card : ScriptableObject
{
    /// <summary>
    /// Value of the card when played
    /// </summary>
    public int Value;

    public Sprite Sprite;

    /// <summary>
    /// Whether this card should be destroyed once it has been played
    /// </summary>
    public bool DestroyOnPlay = true;

    /// <summary>
    /// Whether this card should be destroyed once it has been drawn
    /// </summary>
    public bool DestroyOnDraw;

    /// <summary>
    /// Whether the owner of the deck should know that they have this card.
    /// </summary>
    public bool IsSecret;

    /// <summary>
    /// How much favour this card costs to buy in a shop
    /// </summary>
    public int BuyValue;

    /// <summary>
    /// Delegates to run when the card is drawn
    /// </summary>
    public List<CardEffect> DrawEffects = new();

    /// <summary>
    /// Delegates to run when the card is played
    /// </summary>
    public List<CardEffect> PlayEffects = new();

    /// <summary>
    /// Delegates to run when the card is discarded
    /// </summary>
    public List<CardEffect> DiscardEffects = new();

    public event Action<int> OnValueChanged;
}
