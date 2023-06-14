using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class KeywordManager
{
    public static Dictionary<string, KeyWordInfo> keywords = new()
    {
        {
            "Favour",
            new()
            {
                Description = "The basic bee currency. It can be spent on various things throughout the game",
                Colour = Color.cyan,
            }
        }
    };

    [MenuItem("Tools/Hive of Lies/Populate Keywords Dictionary")]
    private static void PopulateKeywordsDictionary()
    {
        RoleData[] roles = Resources.LoadAll<RoleData>("Roles");

        foreach (RoleData role in roles)
        {
            keywords[role.name] = new()
            {
                Description = $"A {role.Team} role.\nFavour: {role.StartingFavour}\nAbility:{role.Description}",
                Colour = Color.green,
            };
        }
    }

    public static string ModifyKeywordsInString(string str)
    {
        string[] words = str.Split(" ");
        foreach (string word in words)
        {
            //If it's not a keyword, skip it.
            if (!keywords.TryGetValue(word, out KeyWordInfo info)) continue;
            if (info.dirty) continue;
            //Prevent duplicate string replaces
            info.dirty = true;

            //Add formatting
            str = str.Replace(word, $"<link=\"keyword\"><b><color=#{ColorUtility.ToHtmlStringRGB(info.Colour)}>{word}</color></b></link>");
        }

        foreach (KeyValuePair<string, KeyWordInfo> pair in keywords)
        {
            pair.Value.dirty = false;
        }
        return str;
    }
}

public class KeyWordInfo
{
    /// <summary>
    /// The explanation of the keyword
    /// </summary>
    public string Description;
    
    /// <summary>
    /// What colour this keyword will become
    /// </summary>
    public Color Colour;

    public bool dirty;
}