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

        Mission[] missions = Resources.LoadAll<Mission>("Mission/Missions");

        foreach (Mission miss in missions)
        {
            if (miss.name == missionName) return miss;
        }

        Debug.LogError($"Cannot find the mission '{missionName}'. Check it's somewhere in the Scripts/Resources/Missions folder.");
        return null;
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
