using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "HoLPlayerDictionary", menuName = "Variable/Dictionaries/NetworkConnection -> HoLPlayer")]
public class HoLPlayerDictionary : Variable<Dictionary<NetworkConnection, HoLPlayer>>
{
    public new Dictionary<NetworkConnection, HoLPlayer> Value = new Dictionary<NetworkConnection, HoLPlayer>();
}