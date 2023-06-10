using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionCard : MonoBehaviour
{
    Mission Data;

    [SerializeField] TMP_Text missionName;

    [SerializeField] TMP_Text favourCost;

    [SerializeField] GameObject missionEffectPrefab;

    [SerializeField] Transform missionEffectParent;

    [SerializeField] RectTransform cardPos;

    [Tooltip("Whether the local player is alive")]
    [SerializeField] BoolVariable alive;

    public delegate void MissionCardClicked(Mission data);
    public event MissionCardClicked OnMissionCardClicked;

    public void SetData(Mission data, int difficultyMod)
    {
        Data = data;
        missionName.text = Data.MissionName;
        favourCost.text = $"{Data.FavourCost}f";
        foreach (MissionEffectTier tier in data.effects)
        {
            GameObject effect = Instantiate(missionEffectPrefab);
            effect.transform.SetParent(missionEffectParent);

            effect.GetComponent<MissionEffectText>().SetText(tier.Value + difficultyMod + data.DifficultyMod, tier);
        }
    }

    public void SetPos(Vector3 pos)
    {
        cardPos.SetPositionAndRotation(pos, new());
    }

    public void OnClicked()
    {
        if (!alive) return;
        OnMissionCardClicked?.Invoke(Data);
    }
}
