using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionEffectIcon : MonoBehaviour
{
    public TMP_Text Description;
    public Image Icon;
    public Image Background;
    public GameObject Bee;
    public GameObject Wasp;
    public TMP_Text TextOverlay;

    public void CreateIcon(MissionEffect effect)
    {
        if (effect == null || effect.Icon == null)
        {
            Icon.gameObject.SetActive(false);
            return;
        }
        
        Icon.sprite = effect.Icon; 
        Description.text = char.ToUpper(effect.Description[0]) + effect.Description[1..];
        TextOverlay.text = effect.OverlayString;
        Background.color = effect.Colour;
        Bee.SetActive(effect.AffectsBees);
        Wasp.SetActive(effect.AffectsWasps);
    }
}
