using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionUI : MonoBehaviour
{
    [SerializeField] TMP_Text missionName;
    [SerializeField] TMP_Text missionFlavour;
    [SerializeField] TMP_Text successFlavour;
    [SerializeField] TMP_Text successEffect;
    [SerializeField] TMP_Text failFlavour;
    [SerializeField] TMP_Text failEffect;

    private void Start()
    {
        // Listen for server sending us information
    }

    public void ChangeMission(Mission mission)
    {
        missionName.text = mission.Data.Name;
        missionFlavour.text = mission.Data.Description;
        successFlavour.text = mission.Data.SuccessFlavour;
        failFlavour.text = mission.Data.FailFlavour;

        successEffect.text = CreateStringFromList(mission.SuccessEffects);
        failEffect.text = CreateStringFromList(mission.FailEffects);
    }

    /// <summary>
    /// Creates a string from a list of mission effects
    /// </summary>
    /// <param name="list">The list of mission effects</param>
    /// <returns></returns>
    string CreateStringFromList(List<MissionEffect> list)
    {
        string res = "";
        for (int i = 0; i <list.Count; i++)
        {
            res += list[i].Description;
            if (i != list.Count - 1)
            {
                res += ", ";
            }
        }
        return res;
    }
}
