using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public static class NetworkSerialiser
{
    #region RoleData
    public static void WriteRoleData(this NetworkWriter writer, RoleData value)
    {
        writer.WriteString(value.name);
    }
    public static RoleData ReadRoleData(this NetworkReader reader)
    {
        string roleName = reader.ReadString();
        if (roleName == "")
        {
            return null;
        }

        RoleData[] roles = Resources.LoadAll<RoleData>("Roles");

        foreach (RoleData role in roles)
        {
            if (role.name == roleName) return role;
        }

        Debug.LogError($"Cannot find the mission '{roleName}'. Check it's somewhere in the Scripts/Resources/Missions folder.");
        return null;
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

        writer.WriteInt(value.Value);
        writer.WriteSprite(value.Sprite);
        writer.WriteInt(value.BuyValue);
    }

    public static Card ReadCard(this NetworkReader reader)
    {
        Card card = ScriptableObject.CreateInstance<Card>();
        int val = reader.ReadInt();
        card.Value = val;
        card.Sprite = reader.ReadSprite();
        card.BuyValue = reader.ReadInt();
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
        writer.WriteString(value.name);
    }

    public static Sprite ReadSprite(this NetworkReader reader)
    {
        string spriteName = reader.ReadString();
        if (spriteName == "")
        {
            return null;
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");

        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == spriteName) return sprite;
        }

        Debug.LogError($"Cannot find the sprite '{spriteName}'. Check it's somewhere in the Resources/Sprites folder.");
        return null;
    }
    #endregion
}
