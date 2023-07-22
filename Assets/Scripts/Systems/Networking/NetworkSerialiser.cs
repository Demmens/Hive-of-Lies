using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public static class NetworkSerialiser
{
    #region RoleData
    public static void WriteRoleData(this NetworkWriter writer, RoleData value)
    {
        if (value == null)
        {
            writer.WriteString("");
            return;
        }

        //Write path instead of asset bc strings are much less network intensive
        writer.WriteString(AssetDatabase.GetAssetPath(value));
    }
    public static RoleData ReadRoleData(this NetworkReader reader)
    {
        string path = reader.ReadString();
        Debug.Log(path);

        if (path == "") Debug.LogError("Could not find sprite");

        return AssetDatabase.LoadAssetAtPath(path, typeof(RoleData)) as RoleData;
    }
    #endregion

    #region Mission
    public static void WriteMission(this NetworkWriter writer, Mission value)
    {
        if (value == null)
        {
            writer.WriteString("");
            return;
        }
        
        writer.WriteString(value.name);
        writer.WriteInt(value.DifficultyMod);
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
            if (miss.name == missionName)
            {
                miss.DifficultyMod = reader.ReadInt();
                return miss;
            }
        }

        Debug.LogError($"Cannot find the mission '{missionName}'. Check it's somewhere in the Scripts/Resources/Missions folder.");
        return null;
    }
    #endregion

    #region CSteamID
    public static void WriteCSteamID(this NetworkWriter writer, CSteamID value)
    {
        writer.WriteULong(value.m_SteamID);
    }
    public static CSteamID ReadCSteamID(this NetworkReader reader)
    {
        return new CSteamID(reader.ReadULong());
    }
    #endregion

    #region Card
    public static void WriteCard(this NetworkWriter writer, Card value)
    {
        if (value == null)
        {
            Debug.LogError("Trying to serialise a null card");
            return;
        }

        writer.WriteInt(value.TempValue);
        writer.WriteSprite(value.Sprite);
    }

    public static Card ReadCard(this NetworkReader reader)
    {
        Card card = ScriptableObject.CreateInstance<Card>();
        int val = reader.ReadInt();
        card.Value = val;
        card.TempValue = val;
        card.Sprite = reader.ReadSprite();
        return card;
    }
    #endregion

    #region Sprite
    public static void WriteSprite(this NetworkWriter writer, Sprite value)
    {
        if (value == null)
        {
            writer.WriteString("");
            return;
        }

        //Write path instead of asset bc strings are much less network intensive
        writer.WriteString(AssetDatabase.GetAssetPath(value));
    }

    public static Sprite ReadSprite(this NetworkReader reader)
    {
        string path = reader.ReadString();

        if (path == "") Debug.LogError("Could not find sprite");

        return AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
    }
    #endregion
}
