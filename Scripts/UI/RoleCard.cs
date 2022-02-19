using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoleCard : MonoBehaviour
{
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Description;
    [SerializeField] TMP_Text Favour;

    RoleData Data;

    public delegate void RoleCardClicked(RoleData data);
    public event RoleCardClicked OnRoleCardClicked;
    public void SetData(RoleData data)
    {
        Data = data;
        Name.text = Data.RoleName;
        Description.text = Data.Description;
        Favour.text = $"{Data.StartingFavour}f";
    }

    public void OnClicked()
    {
        OnRoleCardClicked?.Invoke(Data);
    }
}
