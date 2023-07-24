using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoleCard : MonoBehaviour
{
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Description;
    [SerializeField] TMP_Text Favour;
    [SerializeField] RectTransform cardPos;
    public GameObject Tutorial;

    RoleData Data;

    public delegate void RoleCardClicked(RoleData data);
    public event RoleCardClicked OnRoleCardClicked;
    public void SetData(RoleData data)
    {
        Data = data;
        Name.text = Data.RoleName.ToUpper();
        Description.text = Data.Description;
        Favour.text = Data.StartingFavour.ToString();
    }

    public void SetPos(Vector3 pos)
    {
        cardPos.SetPositionAndRotation(pos, new Quaternion());
    }

    public void OnClicked()
    {
        OnRoleCardClicked?.Invoke(Data);
    }
}
