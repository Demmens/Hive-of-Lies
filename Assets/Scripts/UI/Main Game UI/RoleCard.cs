using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoleCard : MonoBehaviour
{
    [SerializeField] TMP_Text roleName;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text favour;
    [SerializeField] RectTransform cardPos;
    public GameObject Tutorial;

    [SerializeField] UnityEngine.UI.Image difficulty;
    [SerializeField] Sprite easySprite;
    [SerializeField] Sprite mediumSprite;
    [SerializeField] Sprite hardSprite;

    RoleData Data;

    public delegate void RoleCardClicked(RoleData data);
    public event RoleCardClicked OnRoleCardClicked;
    public void SetData(RoleData data)
    {
        Data = data;
        roleName.text = Data.RoleName.ToUpper();
        description.text = Data.Description;
        favour.text = Data.StartingFavour.ToString();

        difficulty.sprite = GetDifficultySprite(Data.Difficulty);
    }

    public void SetPos(Vector3 pos)
    {
        cardPos.SetPositionAndRotation(pos, new Quaternion());
    }

    public void OnClicked()
    {
        OnRoleCardClicked?.Invoke(Data);
    }

    Sprite GetDifficultySprite(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return easySprite;
            case Difficulty.Medium:
                return mediumSprite;
            case Difficulty.Hard:
                return hardSprite;
            default:
                return easySprite;
        }
    }
}
