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


    public static void WriteMissionData(this NetworkWriter writer, MissionData value)
    {
        writer.WriteString(value.name);
    }
    public static MissionData ReadMissionData(this NetworkReader reader)
    {
        return (MissionData) Resources.Load($"Mission/Missions/{reader.ReadString()}");
    }


    public static void WriteCSteamID(this NetworkWriter writer, CSteamID value)
    {
        writer.WriteULong(value.m_SteamID);
    }
    public static CSteamID ReadCSteamID(this NetworkReader reader)
    {
        return new CSteamID(reader.ReadULong());
    }

    public static void WriteCard(this NetworkWriter writer, Card value)
    {
        if (value == null)
        {
            writer.WriteString("");
            writer.WriteInt(0);
            return;
        }

        writer.WriteString(value.Name);
        writer.WriteInt(value.Value);
    }

    public static Card ReadCard(this NetworkReader reader)
    {
        return new Card(reader.ReadString(), reader.ReadInt());
    }
}
