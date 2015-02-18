using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEditorInternal;

[CustomEditor(typeof(GeneralEvent))]

public class GeneralEventInspector : GameEventInspector
{

    SerializedProperty eventsList;

    private ReorderableList eventsReorderableList;

    private GeneralEvent generalTarget;
    private int componentIndex = 0;
    private int memberIndex;
    private GeneralEventTargetType generalEventTargetType;


    protected override void OnEnable()
    {
        base.OnEnable();

        eventsList = serializedObject.FindProperty("eventsList");

        eventsReorderableList = new ReorderableList(serializedObject, eventsList, true, true, true, true);

        eventsReorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Event Targets");
        };

        eventsReorderableList.elementHeight = 55f;
        eventsReorderableList.drawElementCallback = ListCallback;

        generalTarget = target as GeneralEvent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        eventsReorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    private void ListCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
      //  EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, 3f), new Color(0.25f, 0.25f, 0.25f, 1f));

        var element = eventsReorderableList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2.5f;

       // EditorGUILayout.PropertyField(element);

        // Serialized Properties to work with
        SerializedProperty memberName = element.FindPropertyRelative("memberName");
        SerializedProperty targetType = element.FindPropertyRelative("targetType");
        SerializedProperty targetObj = element.FindPropertyRelative("targetObj");
        SerializedProperty targetComponent = element.FindPropertyRelative("targetComponent");
        SerializedProperty targetValue = element.FindPropertyRelative("targetValue");

        EditorGUI.LabelField(rect, new GUIContent("Element: " + index));

        rect.y += EditorGUIUtility.singleLineHeight;

        DrawTargetObject(rect, index, isActive, isFocused, element, targetObj, targetComponent, targetValue, memberName, targetType);
        DrawTargetMember(rect, index, isActive, isFocused, element, targetComponent, targetValue, memberName, targetType);
    }


    private void DrawTargetObject(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty element, SerializedProperty targetObj, SerializedProperty targetComponent, SerializedProperty targetValue, SerializedProperty memberName, SerializedProperty targetType)
    {
        // Draw the field for the obejct we want to work with
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), targetObj, GUIContent.none);

        componentIndex = 0;

        GameObject targetGO = targetObj.objectReferenceValue as GameObject;
        if (targetGO != null)
        {
            Component[] components = targetGO.GetComponents<Component>();
            GUIContent[] componentContent = new GUIContent[components.Length];
            for (int i = 0; i < componentContent.Length; i++)
            {
                if (components[i] == targetComponent.objectReferenceValue)
                    componentIndex = i;
                componentContent[i] = new GUIContent(components[i].GetType().ToString());
            }

            // Check if the object has changed and if so reset the field index
            int lastIndex = componentIndex;
            componentIndex = EditorGUI.Popup(new Rect(rect.x + 100f, rect.y, rect.width - 100f, EditorGUIUtility.singleLineHeight), componentIndex, componentContent);
            targetComponent.objectReferenceValue = components[componentIndex];

            // Reset the member when the component has changed
            if (lastIndex != componentIndex)
            {
                List<Type> types;
                var mInfo = EventPlusPlusUtility.BuildList(targetComponent.objectReferenceValue as Component, out types);
                if (mInfo.Count > 0)
                {
                    memberName.stringValue = mInfo[0].Name;
                    targetType.stringValue = types[0].ToString();
                  //  SetCurrentValue(mInfo[0], targetValue, targetComponent);
                }
                else
                {
                    memberName.stringValue = "";
                    targetType.stringValue = "";
                }
                memberIndex = 0;
            }
        }
        else
            EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight), new GUIContent("Please select an object"));
    }

    private void DrawTargetMember(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty element, SerializedProperty targetComponent, SerializedProperty targetValue, SerializedProperty memberName, SerializedProperty targetType)
    {
        rect.y += 2f;
        List<Type> types = null;
        List<MemberInfo> mInfo = null;
        if (targetComponent.objectReferenceValue != null)
            mInfo = EventPlusPlusUtility.BuildList((Component)targetComponent.objectReferenceValue, out types);

        // If we can find some members draw the appropriate gui
        if (mInfo != null && mInfo.Count > 0)
        {

            GUIContent[] memberContent = new GUIContent[mInfo.Count];
            for (int i = 0; i < memberContent.Length; i++)
            {
                // If the member name equals, set the member index to that value
                if (mInfo[i].Name.Equals(memberName.stringValue))
                    memberIndex = i;
                memberContent[i] = new GUIContent(mInfo[i].Name + " : " + TypeShortHand(GetGeneralEventTargetType(types[i])));
            }

            generalEventTargetType = GetGeneralEventTargetType(types[memberIndex]);

            // Draw member selection popup
            if (mInfo.Count > 0)
            {
                int lastMemberIndex = memberIndex;
                memberIndex = EditorGUI.Popup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, 100f, EditorGUIUtility.singleLineHeight), memberIndex, memberContent);
                // If member index changed
                if (lastMemberIndex != memberIndex)
                {
                    generalEventTargetType = GetGeneralEventTargetType(types[memberIndex]);
                    SetCurrentValue(mInfo[memberIndex], targetValue, targetComponent);
                    memberName.stringValue = mInfo[memberIndex].Name;
                    targetType.stringValue = types[memberIndex].ToString();
                }
            }

            // Draw target value type
            DrawTypeInput(generalEventTargetType, targetValue, new Rect(rect.x + 100, rect.y + EditorGUIUtility.singleLineHeight, rect.width - 100f - 120f, EditorGUIUtility.singleLineHeight));

            if (GUI.Button(new Rect(rect.x + rect.width - 120f, rect.y + EditorGUIUtility.singleLineHeight, 120f, EditorGUIUtility.singleLineHeight), new GUIContent("Get Current Value")))
            {
                SetCurrentValue(mInfo[memberIndex], targetValue, targetComponent);
            }

        }
        //If we can't find any members inform the user no members exisit
        else
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("No applicable fields could be found"));
        }

    }

    /// <summary>
    /// Get the generalEvenTargetType value for ease of access
    /// </summary>
    /// <param name="type"></param>
    private GeneralEventTargetType GetGeneralEventTargetType(Type type)
    {
        if (type == typeof(float))
            return GeneralEventTargetType.Float;
        else if (type == typeof(int))
            return GeneralEventTargetType.Int;
        else if (type == typeof(Vector3))
            return GeneralEventTargetType.Vector3;
        else if (type == typeof(Color))
            return GeneralEventTargetType.Color;
        else
            return GeneralEventTargetType.None;
    }

    private void SetCurrentValue(MemberInfo selectedMember, SerializedProperty targetValue, SerializedProperty targetComponent)
    {
        object memValue = GetMemberValue(selectedMember, targetComponent);
        switch (generalEventTargetType)
        {
            case GeneralEventTargetType.Vector3:
                targetValue.FindPropertyRelative("vector3Value").vector3Value = (Vector3)memValue;
                break;
            case GeneralEventTargetType.Float:
                targetValue.FindPropertyRelative("floatValue").floatValue = (float)memValue;
                break;
            case GeneralEventTargetType.Color:
                targetValue.FindPropertyRelative("colorValue").colorValue = (Color)memValue;
                break;
        }
    }

    private object GetMemberValue(MemberInfo info, SerializedProperty targetComponent)
    {
        if (info is PropertyInfo)
            return ((PropertyInfo)info).GetValue(targetComponent.objectReferenceValue, null);
        else if (info is FieldInfo)
            return ((FieldInfo)info).GetValue(targetComponent.objectReferenceValue);
        else
            return null;
    }

    private void DrawTypeInput(GeneralEventTargetType type, SerializedProperty targetValue, Rect rect)
    {
        switch (type)
        {
            case GeneralEventTargetType.Float:
                targetValue.FindPropertyRelative("floatValue").floatValue = EditorGUI.FloatField(rect, targetValue.FindPropertyRelative("floatValue").floatValue);
                break;
            case GeneralEventTargetType.Int:
                EditorGUI.IntField(rect, 0);
                break;
            case GeneralEventTargetType.Color:
                targetValue.FindPropertyRelative("colorValue").colorValue = EditorGUI.ColorField(rect, targetValue.FindPropertyRelative("colorValue").colorValue);
                break;
            case GeneralEventTargetType.Vector3:
                targetValue.FindPropertyRelative("vector3Value").vector3Value = EditorGUI.Vector3Field(rect, new GUIContent(""), targetValue.FindPropertyRelative("vector3Value").vector3Value);
                break;
            case GeneralEventTargetType.None:
                EditorGUILayout.LabelField(new GUIContent("Error field not selected"));
                break;
        }
    }

    private string TypeShortHand(GeneralEventTargetType type)
    {
        switch (type)
        {
            case GeneralEventTargetType.Float:
                return "float";
            case GeneralEventTargetType.Int:
                return "int";
            case GeneralEventTargetType.Color:
                return "color";
            case GeneralEventTargetType.Vector3:
                return "vector3";
            case GeneralEventTargetType.None:
            default:
                return "ERROR";
        }
    }

}

enum GeneralEventTargetType
{
    None,
    Float,
    Int,
    Vector3,
    Color
}