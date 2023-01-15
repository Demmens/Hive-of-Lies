using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Mission", menuName = "Missions/Create mission")]
public class Mission : ScriptableObject
{
    #region Fields
    /// <summary>
    /// Private counterpart to <see cref="MissionName"/>
    /// </summary>
    [SerializeField] string missionName;

    /// <summary>
    /// Private counterpart to <see cref="Description"/>
    /// </summary>
    [Multiline]
    [SerializeField] string description;

    /// <summary>
    /// Private counterpart to <see cref="FavourCost"/>
    /// </summary>
    [SerializeField] int favourCost;

    [Space]
    [Space]

    [SerializeField] public List<MissionEffectTier> effects;

    [Space]
    [Space]

    /// <summary>
    /// Private counterpart to <see cref="Condition"/>
    /// </summary>
    [SerializeField] MissionCondition condition;

    //int tiersTriggered;
    #endregion

    #region Properties

    /// <summary>
    /// Name of the mission
    /// </summary>
    public string MissionName
    {
        get
        {
            return missionName;
        }
    }

    /// <summary>
    /// The flavour text for the mission
    /// </summary>
    public string Description
    {
        get
        {
            return description;
        }
    }
    /// <summary>
    /// How much it costs to stand for the teamLeader of this mission
    /// </summary>
    public int FavourCost
    {
        get
        {
            return favourCost;
        }
    }

    /// <summary>
    /// The condition under which this mission will appear
    /// </summary>
    public MissionCondition Condition
    {
        get
        {
            return condition;
        }
    }
    #endregion

    public event System.Action AfterAllEffectsTriggered;

    public void TriggerValidEffects(int cardsTotal)
    {
        int tiersTriggered = 0;
        int applicableTiers = effects.Count(tier => tier.Applicable(cardsTotal));

        foreach (MissionEffectTier tier in effects)
        {
            if (!tier.Applicable(cardsTotal)) continue;
            tier.AfterEffectsTriggered += () =>
            {
                if (++tiersTriggered >= applicableTiers) AfterAllEffectsTriggered?.Invoke();
            };

            tier.ApplyEffects(cardsTotal);
        }
    }
}

[System.Serializable]
public class MissionEffectTier
{
    public Comparator comparator;
    public int value;
    public string effectFlavour;
    public List<MissionEffect> effects;
    [Multiline]

    int effectsTriggered;

    public event System.Action AfterEffectsTriggered;

    public bool Applicable(int cardsTotal)
    {
        if (comparator == Comparator.GreaterThan) return cardsTotal > value;

        if (comparator == Comparator.EqualTo) return cardsTotal == value;

        if (comparator == Comparator.LessThan) return cardsTotal < value;

        Debug.LogError("Mission effect tier has an unknown comparator");
        return false;
    }

    public void ApplyEffects(int cardsTotal)
    {
        if (!Applicable(cardsTotal)) return;

        effects.ForEach(effect => {
            effect.OnMissionEffectFinished += EffectTriggered;
            effect.TriggerEffect();
        });
    }

    public void EffectTriggered(MissionEffect effect)
    {
        effect.OnMissionEffectFinished -= EffectTriggered;
        if (++effectsTriggered >= effects.Count)
        {
            AfterEffectsTriggered?.Invoke();
        }
    }
}

public enum Comparator
{
    GreaterThan,
    EqualTo,
    LessThan
}