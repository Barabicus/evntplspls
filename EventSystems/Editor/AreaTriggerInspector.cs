using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CanEditMultipleObjects]
[CustomEditor(typeof(AreaTrigger))]
public class AreaTriggerInspector : Editor
{
    private SerializedProperty triggerableObjects;
    private AreaTrigger targetTrigger;
    private GameObject newObj;
    private ReorderableList list;


    void OnEnable()
    {
        triggerableObjects = serializedObject.FindProperty("triggerableObjects");
        targetTrigger = target as AreaTrigger;

        list = new ReorderableList(serializedObject, triggerableObjects,
true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Triggerable Objects");
        };

        list.drawElementCallback =
    (Rect rect, int index, bool isActive, bool isFocused) =>
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
            element, GUIContent.none);
    };

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        if (targetTrigger.triggeredBy == AreaTrigger.TriggeredBy.Select)
                    list.DoLayoutList();

           // DrawTriggerObjectsArray();


        serializedObject.ApplyModifiedProperties();
    }

    void DrawTriggerObjectsArray()
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField(new GUIContent("Triggerable Entities"));
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < triggerableObjects.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal("Box");
            GameObject gObj = triggerableObjects.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
            if (gObj == null)
            {
                triggerableObjects.DeleteArrayElementAtIndex(i);
                triggerableObjects.DeleteArrayElementAtIndex(i);
            }
            else
                EditorGUILayout.PropertyField(triggerableObjects.GetArrayElementAtIndex(i));
            if (GUILayout.Button(new GUIContent("[-]")))
            {
                triggerableObjects.DeleteArrayElementAtIndex(i);
                triggerableObjects.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        newObj = EditorGUILayout.ObjectField(new GUIContent("Add Object"), newObj, typeof(GameObject), true) as GameObject;

        if (newObj != null)
        {
            int index = triggerableObjects.arraySize;
            triggerableObjects.InsertArrayElementAtIndex(index);
            SerializedProperty spellProp = triggerableObjects.GetArrayElementAtIndex(index);
            spellProp.objectReferenceValue = newObj;
            newObj = null;
        }

        EditorGUILayout.EndVertical();

    }

}
