using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class PlayerButton : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text playerNameText;
    [SerializeField] UnityEngine.UI.RawImage exhaustionUI;

    [SerializeField] Texture someExhaustion;
    [SerializeField] Texture heavyExhaustion;

    [SerializeField] GameObject isReady;
    [SerializeField] GameObject exhaustionObj;
    /// <summary>
    /// The ID of the player associated with this button
    /// </summary>
    [HideInInspector]
    public ulong ID;

    private string playerName;
    /// <summary>
    /// ID of the player associated with this button
    /// </summary>
    public string PlayerName
    {
        get
        {
            return playerName;
        }
        set
        {
            playerName = value;
            playerNameText.text = value;
        }
    }

    /// <summary>
    /// Whether this is selected or not
    /// </summary>
    [HideInInspector]
    public bool selected;

    /// <summary>
    /// The PlayerButtonDropdownItems that appear when you click this button
    /// </summary>
    [HideInInspector]
    public List<GameObject> listItems = new List<GameObject>();

    [Tooltip("Invoked when a button is clicked")]
    [SerializeField] UlongEvent onClicked;

    public void Click()
    {
        onClicked?.Invoke(ID);
    }

    public void SetReady(bool ready)
    {
        isReady.SetActive(ready);
    }

    public void ChangeExhaustion(int exhaustion)
    {
        if (exhaustion == 0) exhaustionObj.SetActive(false);
        else exhaustionObj.SetActive(true);

        if (exhaustion == 1) exhaustionUI.texture = someExhaustion;
        if (exhaustion == 2) exhaustionUI.texture = heavyExhaustion;
    }
}
