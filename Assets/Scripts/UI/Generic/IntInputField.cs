using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEditor;
using System.Reflection;

public class IntInputField : TMP_InputField
{
    public UnityEvent<int> OnIntValueChanged;
    public int MaxValue = 1000;
    int oldVal;
    public int intText 
    { 
        get 
        { 
            return int.Parse(text);
        }
        set
        {
            text = value.ToString();
        }
    }

    new void Start()
    {
        int.TryParse(text, out oldVal);
        onValueChanged.AddListener(RemoveLetters);
    }

    void RemoveLetters(string input)
    {
        //if the full input is a number
        if (int.TryParse(input, out int result))
        {
            if (result > MaxValue) result = oldVal;
            oldVal = result;
            //In case the number starts with 0
            SetTextWithoutNotify(result.ToString());
            OnIntValueChanged.Invoke(result);
            return;
        }

        char[] chars = input.ToCharArray();

        string finalString = "";

        for (int i = 0; i < chars.Length; i++)
        {
            if (!int.TryParse(chars[i].ToString(), out result)) continue;

            finalString += chars[i];
        }

        finalString = finalString.TrimStart('0');

        if (finalString == "")
        {
            SetTextWithoutNotify("0");
            OnIntValueChanged.Invoke(result);
            m_StringPosition = 1;
            m_StringSelectPosition = 1;
            return;
        }

        if (!int.TryParse(finalString, out result))
        {
            Debug.LogError("Somehow string ended up as NaN");
            return;
        }

        if (result > MaxValue) result = oldVal;
        oldVal = result;
        SetTextWithoutNotify(result.ToString());
        OnIntValueChanged.Invoke(result);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(IntInputField))]
class IntInputFieldEditor : Editor
{
    Editor defaultEditor;
    SerializedProperty eventProp;
    private void OnEnable()
    {
        defaultEditor = CreateEditor(targets, typeof(TMPro.EditorUtilities.TMP_InputFieldEditor));
        eventProp = serializedObject.FindProperty(nameof(IntInputField.OnIntValueChanged));
    }

    void OnDisable()
    {
        //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
        //Also, make sure to call any required methods like OnDisable
        MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (disableMethod != null)
            disableMethod.Invoke(defaultEditor, null);
        DestroyImmediate(defaultEditor);
    }

    public override void OnInspectorGUI()
    {
        defaultEditor.OnInspectorGUI();
        IntInputField inputField = target as IntInputField;

        EditorGUILayout.PropertyField(eventProp);
        inputField.MaxValue = EditorGUILayout.IntField("Max Value", inputField.MaxValue);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif