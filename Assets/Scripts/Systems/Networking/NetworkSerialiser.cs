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


    public static void WriteMission(this NetworkWriter writer, Mission value)
    {
        if (value == null)
        {
            writer.WriteString("");
            return;
        }
        
        writer.WriteString(value.name);
    }
    public static Mission ReadMission(this NetworkReader reader)
    {
        string missionName = reader.ReadString();
        if (missionName == "")
        {
            return null;
        }
        if ((Mission) Resources.Load($"Mission/Missions/{missionName}") == null) {
            Debug.LogError("Cannot find the mission for some reason. Check it's in the correct folder.");
            return ScriptableObject.CreateInstance<Mission>();
        }

        return (Mission) Resources.Load($"Mission/Missions/{missionName}");
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
        writer.WriteInt(value.TempValue);
    }

    public static Card ReadCard(this NetworkReader reader)
    {
        return new Card(reader.ReadString(), reader.ReadInt());
    }
}
