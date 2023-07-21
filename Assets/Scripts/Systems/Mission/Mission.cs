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

    [HideInInspector] public int DifficultyMod;
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

    public event System.Action AfterEffectTriggered;
    public event System.Action<EMissionPlotPoint> OnPlotPointTraversed;

    public void TriggerValidEffects(int cardsTotal)
    {
        cardsTotal -= DifficultyMod;

        MissionEffectTier tier = GetValidEffect(cardsTotal);

        tier.AfterEffectsTriggered += () =>
        {
            AfterEffectTriggered?.Invoke();
        };

        foreach (EMissionPlotPoint point in tier.plotPoints)
        {
            OnPlotPointTraversed?.Invoke(point);
        }

        tier.ApplyEffects();
    }

    public MissionEffectTier GetValidEffect(int cardsTotal)
    {
        return effects[GetValidTier(cardsTotal)];
    }

    public int GetValidTier(int cardsTotal)
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            MissionEffectTier tier = effects[i];

            if (!tier.Applicable(cardsTotal) && i > 0) continue;

            return i;
        }

        //By default the first tier should ALWAYS be an option
        return 0;
    }
}

[System.Serializable]
public class MissionEffectTier
{
    [SerializeField] int value;
    public int Value
    {
        get
        {
            return value;
        }
    }
    public string effectFlavour;
    public List<MissionEffect> effects;
    public List<EMissionPlotPoint> plotPoints;

    int effectsTriggered;

    public event System.Action AfterEffectsTriggered;

    public bool Applicable(int cardsTotal)
    {
        return cardsTotal >= value;
    }

    public void ApplyEffects()
    {
        if (effects.Count == 0) AfterEffectsTriggered?.Invoke();

        foreach (MissionEffect effect in effects)
        {
            effect.OnMissionEffectFinished += EffectTriggered;
            effect.TriggerEffect();
        }
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