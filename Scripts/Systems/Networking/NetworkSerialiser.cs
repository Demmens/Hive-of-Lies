using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public static class NetworkSerialiser
{
    public static void WriteRoleData(this NetworkWriter writer, RoleData value)
    {
        writer.WriteString(value.name);
    }
    public static RoleData ReadRoleData(this NetworkReader reader)
    {
        return Resources.Load($"Roles/{reader.ReadString()}") as RoleData;
    }


    public static void ReadMissionData(this NetworkWriter writer, MissionData value)
    {
        writer.WriteString(value.name);
    }
    public static MissionData ReadMissionData(this NetworkReader reader)
    {
        return (MissionData) Resources.Load($"Mission/Missions/{reader.ReadString()}");
    }


    public static void ReadCSteamID(this NetworkWriter writer, CSteamID value)
    {
        writer.WriteULong(value.m_SteamID);
    }
    public static CSteamID ReadCSteamID(this NetworkReader reader)
    {
        return new CSteamID(reader.ReadULong());
    }
}
