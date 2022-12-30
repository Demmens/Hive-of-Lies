using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    /// <summary>
    /// The name of the card
    /// </summary>
    public string Name;
    /// <summary>
    /// Value of the card when played
    /// </summary>
    public int Value;
    /// <summary>
    /// The value of the card after all modifiers are applied. Resets back to the default value when discarded.
    /// </summary>
    public int TempValue;
    
    /// <summary>
    /// Delegates to run when the card is drawn
    /// </summary>
    public List<Action> DrawEffects = new List<Action>();

    /// <summary>
    /// Delegates to run when the card is played
    /// </summary>
    public List<Action> PlayEffects = new List<Action>();

    /// <summary>
    /// Delegates to run when the card is discarded
    /// </summary>
    public List<Action> DiscardEffects = new List<Action>();
 

    public Card(int value)
    {
        Value = value;
        TempValue = value;
    }

    public Card(string name, int value)
    {
        Name = name;
        Value = value;
        TempValue = value;
    }
}
