using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

namespace HiveOfLies
{
    public static class Functions
    {
        public static NetworkConnection FindConnectionFromPlayer(Player ply)
        {
            return GameInfo.Players.FirstOrDefault(x => x.Value == ply).Key;
        }
    }
}

