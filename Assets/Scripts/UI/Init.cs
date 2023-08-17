using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class Init : MonoBehaviour
{
    [SerializeField] GameObject consentPopup;
    const string consentString = "Consent to analytics";
    const string consentPopupString = "Shown consent popup";
    async void Start()
    {
        //Temporary measure to prevent bad UI scaling while sending to publishers. UI designer will be hired to fix this (I'm so sorry) :)
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

        await UnityServices.InitializeAsync();

        if (PlayerPrefs.GetInt(consentString) > 0) Consent(true);
        if (PlayerPrefs.GetInt(consentPopupString) > 0) return;

        consentPopup.SetActive(true);
        PlayerPrefs.SetInt(consentPopupString, 1);
    }

    public void Consent(bool consent)
    {
        if (consent)
        {
            PlayerPrefs.SetInt(consentString, 1);
            AnalyticsService.Instance.StartDataCollection();
        }
        else
        {
            PlayerPrefs.SetInt(consentString, 0);
            AnalyticsService.Instance.StopDataCollection();
        }
    }

    public void DeleteData()
    {
        AnalyticsService.Instance.RequestDataDeletion();
    }
}
