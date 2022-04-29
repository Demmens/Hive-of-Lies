using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FavourController : MonoBehaviour
{
    public static FavourController singleton;
    [SerializeField] TMPro.TMP_Text FavourText;
    private int favour;

    public int Favour
    {
        get
        {
            return favour;
        }
        set
        {
            favour = value;
            FavourText.text = $"{value}f";
        }
    }

    private void Start()
    {
        singleton = this;
        NetworkClient.RegisterHandler<ChangeFavourMsg>(ChangeFavour);
        NetworkClient.RegisterHandler<SetFavourMsg>(SetFavour);
    }

    void ChangeFavour(ChangeFavourMsg msg)
    {
        Favour += msg.favourIncrease;
    }

    void SetFavour(SetFavourMsg msg)
    {
        Favour = msg.newFavour;
    }
}

public struct ChangeFavourMsg : NetworkMessage
{
    public int favourIncrease;
}

public struct SetFavourMsg : NetworkMessage
{
    public int newFavour;
}