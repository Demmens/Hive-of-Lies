using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public abstract class MissionEffect : ScriptableObject
{
    [Tooltip("Description of the effect")]
    [SerializeField] LocalizedString description;

    public delegate void MissionEffectOver(MissionEffect effect);
    /// <summary>
    /// Invoked when the mission effect has finished. Important for effects that aren't instantaneous (e.g. investigation)
    /// </summary>
    public event MissionEffectOver OnMissionEffectFinished;

    /// <summary>
    /// Description of the effect
    /// </summary>
    public string Description
    {
        get
        {
            return description.GetLocalizedString();
        }
    }

    [Tooltip("The icon of this effect")]
    [field:SerializeField]
    public Sprite Icon { get; private set; }

    [Tooltip("Whether to show the Bee icon on the effect hex")]
    [field:SerializeField]
    public bool AffectsBees { get; private set; }

    [Tooltip("Whether to show the Wasp icon on the effect hex")]
    [field: SerializeField]
    public bool AffectsWasps { get; private set; }

    [Tooltip("Whether this is considered a positive, negative, or neutral effect for the Bees")]
    [field:SerializeField] 
    public EffectType Type { get; private set; }

    [Tooltip("The colour that the effect hex should be")]
    [field:SerializeField]
    public ColourVariable Colour { get; private set; }

    [Tooltip("The text to overlay on the effect icon. Should be no longer than 2 characters.")]
    [field:SerializeField]
    public string OverlayString { get; private set; }

    /// <summary>
    /// Trigger this mission effect
    /// </summary>
    public abstract void TriggerEffect();

    /// <summary>
    /// Call on the server to end the mission effect
    /// </summary>
    protected void EndEffect()
    {
        OnMissionEffectFinished?.Invoke(this);
    }

    public enum EffectType
    {
        Positive,
        Negative,
        Neutral,
    }
}
