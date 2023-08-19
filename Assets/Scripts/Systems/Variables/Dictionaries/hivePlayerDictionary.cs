using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Hive Player Dictionary", menuName = "Variable/Dictionaries/NetworkConnection -> hivePlayer")]
public class hivePlayerDictionary : Variable<Dictionary<NetworkConnection, hivePlayer>>
{
    public new Dictionary<NetworkConnection, hivePlayer> Value = new Dictionary<NetworkConnection, hivePlayer>();
}