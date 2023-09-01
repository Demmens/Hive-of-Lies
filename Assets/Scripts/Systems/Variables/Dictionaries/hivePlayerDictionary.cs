using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Hive Player Dictionary", menuName = "Variable/Dictionaries/NetworkConnection -> hivePlayer")]
public class HivePlayerDictionary : Variable<Dictionary<NetworkConnection, HivePlayer>>
{
    public new Dictionary<NetworkConnection, HivePlayer> Value = new Dictionary<NetworkConnection, HivePlayer>();
}