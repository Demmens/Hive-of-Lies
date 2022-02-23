using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionCard : MonoBehaviour
{
    MissionData Data;

    [SerializeField] TMP_Text Name;

    [SerializeField] TMP_Text SuccessEffect;

    [SerializeField] TMP_Text FailEffect;

    [SerializeField] TMP_Text FavourCost;

    public delegate void MissionCardClicked(MissionData data);
    public event MissionCardClicked OnMissionCardClicked;

    public void SetData(MissionData data)
    {
        Data = data;
        Debug.Log(Data.MissionName);
        Name.text = Data.MissionName;
        FavourCost.text = $"{Data.FavourCost}f";
        SuccessEffect.text = MissionUI.CreateStringFromList(Data.SuccessEffects);
        FailEffect.text = MissionUI.CreateStringFromList(Data.FailEffects);
    }

    public void OnClicked()
    {
        OnMissionCardClicked?.Invoke(Data);
    }
}
