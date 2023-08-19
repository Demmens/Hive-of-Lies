using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class MissionUI : NetworkBehaviour
{
    #region CLIENT
    [SerializeField] GameObject missionUI;
    [SerializeField] GameObject missionBar;
    [SerializeField] TMP_Text missionName;
    [SerializeField] TMP_Text missionFlavour;

    [SerializeField] List<GameObject> effects;
    [SerializeField] List<TMP_Text> effectRequirements;
    [SerializeField] List<Transform> effectIcons;

    [SerializeField] GameObject iconPrefab;

    [SerializeField] Image missionFill;

    [Tooltip("Returns true if the player is on the mission")]
    [SerializeField] BoolVariable isOnMission;

    List<GameObject> currentEffectIcons = new();
    #endregion

    #region SERVER

    [Tooltip("How much more difficult all missions are")]
    [SerializeField] IntVariable missionDifficulty;

    [Tooltip("The mission that is currently active")]
    [SerializeField] MissionVariable currentMission;

    [Tooltip("The total value of all played cards")]
    [SerializeField] IntVariable cardsTotal;

    [Tooltip("The current team leader")]
    [SerializeField] hivePlayerVariable teamLeader;

    [Tooltip("All players that have currently been selected to go on the mission")]
    [SerializeField] hivePlayerSet playersSelected;

    [Tooltip("Invoked after the mission bar has filled to the max")]
    [SerializeField] GameEvent afterMissionResultShown;
    #endregion

    public override void OnStartServer()
    {
        currentMission.AfterVariableChanged += miss => ChangeMission(miss, missionDifficulty);
        teamLeader.AfterVariableChanged += ply =>
        {
            if (ply == null) return;
            GiveTeamLeader(ply.connectionToClient);
        };
        playersSelected.AfterItemAdded += ply => OnPartnerChange(ply.DisplayName, ply.PlayerID, true);
        playersSelected.AfterItemRemoved += ply => OnPartnerChange(ply.DisplayName, ply.PlayerID, false);
    }

    /// <summary>
    /// Creates a string from a list of mission effects
    /// </summary>
    /// <param name="list">The list of mission effects</param>
    /// <returns></returns>
    public static string CreateStringFromList(List<MissionEffect> list)
    {
        string res = "";
        for (int i = 0; i < list.Count; i++)
        {
            res += list[i].Description;
            if (i != list.Count - 1)
            {
                res += ", ";
            }
        }
        if (res == "") res = "No effect";
        return res;
    }

    [ClientRpc]
    void ChangeMission(Mission mission, int difficultyMod)
    {
        //Clear out all currently existing effect icons from the mission screen
        foreach (GameObject icon in currentEffectIcons) Destroy(icon);
        currentEffectIcons = new();
        //Clear mission bar fill
        missionFill.fillAmount = 0;

        if (mission == null) return;
        missionUI.SetActive(true);
        missionName.text = mission.MissionName;
        missionName.gameObject.SetActive(true);
        missionFlavour.text = mission.Description;
        missionFlavour.gameObject.SetActive(true);
        missionBar.SetActive(true);

        //Set these two effects manually because they need to be the effects at either side of the bar
        SetEffect(mission, mission.effects[0], difficultyMod, 0);
        SetEffect(mission, mission.effects[mission.effects.Count -1 ], difficultyMod, 3);

        //All other effects can be put in the middle
        for (int i = 1; i < effects.Count-1; i++) {

            if (i >= mission.effects.Count-1)
            {
                effects[i].SetActive(false);
                continue;
            }

            effects[i].SetActive(true);

            MissionEffectTier tier = mission.effects[i];

            SetEffect(mission, tier, difficultyMod, i);
        }
    }

    [Client]
    void SetEffect(Mission mission, MissionEffectTier tier, int difficultyMod, int effectIndex)
    {
        for (int i = 0; i < tier.effects.Count; i++)
        {
            MissionEffect eff = tier.effects[i];

            if (eff.Icon == null) continue;

            GameObject icon = Instantiate(iconPrefab);
            icon.transform.SetParent(effectIcons[effectIndex]);
            MissionEffectIcon iconScript = icon.GetComponentInChildren<MissionEffectIcon>();
            iconScript.Icon.sprite = eff.Icon;
            iconScript.Description.text = eff.Description;
            currentEffectIcons.Add(icon);
        }

        if (effectIndex > 0) effectRequirements[effectIndex].text = (tier.Value + difficultyMod + mission.DifficultyMod).ToString();
    }

    //TODO: Move this to a more appropriate place
    [TargetRpc]
    void GiveTeamLeader(NetworkConnection conn)
    {
        isOnMission.Value = true;
    }

    //TODO: Move this to a more appropriate place
    [ClientRpc]
    void OnPartnerChange(string name, ulong ID, bool added)
    {
        if (ID != (ulong)SteamUser.GetSteamID()) return;
        isOnMission.Value = added;
    }

    [Server]
    public void AfterCardsPlayed()
    {
        int index = currentMission.Value.GetValidTier(cardsTotal - missionDifficulty);
        int max = currentMission.Value.effects.Count - 1;
        ShowMissionResult(index, max);
        StartCoroutine(Coroutines.Delay(5, afterMissionResultShown.Invoke));
    }

    [ClientRpc]
    public void ShowMissionResult(int resultIndex, int resultMax)
    {
        float finalFill = (float)resultIndex / resultMax;

        //Show at least *some* progress, even if the mission fails.
        if (finalFill == 0) finalFill = 0.075f;

        StartCoroutine(FillBar(finalFill, 4));
    }

    IEnumerator FillBar(float finalFill, float seconds)
    {
        float time = 0;
        
        while (time < seconds)
        {
            time += Time.deltaTime;
            float t = time / seconds;

            missionFill.fillAmount = Easing.GradualEnd(t) * finalFill;
            yield return null;
        }
    }
}