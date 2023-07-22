using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Information about the role
/// </summary>
[CreateAssetMenu(fileName = "RoleData", menuName = "Roles/Create role")]
public class RoleData : ScriptableObject
{
    #region Private Properties
    /// <summary>
    /// Private counterpart to <see cref="StartingDeck"/>
    /// </summary>
    [SerializeField] List<Card> startingDeck;

    /// <summary>
    /// Prefab containing RoleAbility scripts
    /// </summary>
    [SerializeField] List<GameObject> abilities;

    /// <summary>
    /// Name of the role
    /// </summary>
    public string RoleName { get; private set; }

    /// <summary>
    /// Description for the role
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Sprite used for the role UI
    /// </summary>
    public Sprite Sprite { get; private set; }

    /// <summary>
    /// Amount of influence the role starts with
    /// </summary>
    public int StartingFavour { get; private set; }

    /// <summary>
    /// The team the role belongs to
    /// </summary>
    public Team Team { get; private set; }

    /// <summary>
    /// The type of wasp role this is
    /// </summary>
    public WaspType WaspType { get; private set; }

    /// <summary>
    /// The minimum number of players required in the lobby for this role to appear
    /// </summary>
    public int PlayersRequired { get; private set; }

    /// <summary>
    /// The logic for the roles ability
    /// </summary>
    public List<GameObject> Abilities { get; private set; }

    /// <summary>
    /// Whether the role should appear in games
    /// </summary>
    public bool Enabled { get; private set; }

    /// <summary>
    /// The roles starting deck of cards
    /// </summary>
    public List<Card> StartingDeck { get; private set; }
    #endregion

    #region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(RoleData))]
    public class RoleDataEditor : Editor
    {
        bool basicInfo = true;

        public override void OnInspectorGUI()
        {
            RoleData role = (RoleData) target;

            
            basicInfo = EditorGUILayout.BeginFoldoutHeaderGroup(basicInfo, "Basic Info");

            if (basicInfo)
            {
                EditorGUILayout.LabelField("Name");
                role.RoleName = EditorGUILayout.TextField(role.RoleName);
                EditorGUILayout.LabelField("Description");
                role.Description = EditorGUILayout.TextArea(role.Description, GUILayout.Height(50));
                role.Team = (Team)EditorGUILayout.EnumPopup("Team", role.Team);
                if (role.Team == Team.Wasp) role.WaspType = (WaspType)EditorGUILayout.EnumPopup("Wasp Type", role.WaspType);
                role.StartingFavour = EditorGUILayout.IntField("Favour", role.StartingFavour);
                role.PlayersRequired = EditorGUILayout.IntField("Player Count", role.PlayersRequired);
                role.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", role.Sprite, typeof(Sprite), false);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            base.OnInspectorGUI();

            role.Enabled = EditorGUILayout.Toggle("Enabled", role.Enabled);
        }
    }
#endif
    #endregion
}

public enum WaspType
{
    Solitary,
    Social
}