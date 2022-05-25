using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class GamblerAbility : RoleAbility
{
    /*
    DiceMission dice;
    CostCalculation costCalc;

    List<int> previousRolls;
    bool goneBust = false;

    void Start()
    {
        previousRolls = new List<int>();
        dice = FindObjectOfType<DiceMission>();
        costCalc = FindObjectOfType<CostCalculation>();

        dice.OnPlayerRolled += PlayerRolled;
        costCalc.OnRerollCalculation += ModifyRerollCost;
        dice.OnMissionEnded += MissionEnd;

        NetworkClient.RegisterHandler<GoneBustMsg>(ClientGoneBust);
        NetworkServer.RegisterHandler<TestNetworkingMsg>(TestNetworking);
    }

    public override void OnStartLocalPlayer()
    {
        NetworkClient.Send(new TestNetworkingMsg() { authority = hasAuthority });
    }

    void TestNetworking(NetworkConnection conn, TestNetworkingMsg msg)
    {
        Debug.Log($"player has authority: {msg.authority}");
    }

    /// <summary>
    /// If bust, the next reroll costs 99 favour
    /// </summary>
    /// <param name="ply">Player rerolling</param>
    /// <param name="cost">Cost of the reroll</param>
    void ModifyRerollCost(CSteamID ply, ref int cost)
    {
        if (ply != Owner.SteamID || !hasAuthority) return;

        cost = goneBust ? 999 : 0;
    }

    /// <summary>
    /// Server version of player rolled
    /// </summary>
    /// <param name="ply">The player that rolled</param>
    /// <param name="roll">The roll information</param>
    void PlayerRolled(Player ply, ref PlayerRollInfo roll)
    {
        if (ply != Owner) return;
        CheckForBust(roll.currentRoll);
    }

    /// <summary>
    /// Check if the roll makes the player go bust
    /// </summary>
    /// <param name="roll">The new roll</param>
    void CheckForBust(int roll)
    {
        if (previousRolls.Contains(roll))
        {
            GoBust();
            OwnerConnection.Send(new GoneBustMsg { });
            return;
        }

        previousRolls.Add(roll);
    }

    /// <summary>
    /// Server tells the client they've gone bust
    /// </summary>
    void ClientGoneBust(GoneBustMsg msg)
    {
        GoBust();
    }

    /// <summary>
    /// Logic for what immediately happens on going bust
    /// </summary>
    void GoBust()
    {
        goneBust = true;
    }

    /// <summary>
    /// Reset previous rolls on mission end
    /// </summary>
    void MissionEnd(MissionResult result)
    {
        previousRolls = new List<int>();
        goneBust = false;
    }
    */
}

struct GoneBustMsg : NetworkMessage
{

}

struct TestNetworkingMsg : NetworkMessage 
{
    public bool authority;
}