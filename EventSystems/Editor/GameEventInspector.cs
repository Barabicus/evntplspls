using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(GameEvent), true)]
public class GameEventInspector : Editor
{

    private ReorderableList list;
    private SerializedProperty triggers;

    protected virtual void OnEnable()
    {
        triggers = serializedObject.FindProperty("triggers");
        list = new ReorderableList(serializedObject, triggers, true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Triggers");
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
        base.OnInspectorGUI();
        serializedObject.Update();
        list.DoLayoutList();

        Transform obj = (Transform)EditorGUILayout.ObjectField(new GUIContent("Search for and add Triggers", "Search this object and all its children for triggers and add them to the trigger list"), null, typeof(Transform));

        // If an object has been added look through it and its children for triggers
        if (obj != null)
        {
            foreach (GameEventTrigger trigger in obj.GetComponentsInChildren<GameEventTrigger>())
            {
                int index = triggers.arraySize;
                triggers.InsertArrayElementAtIndex(triggers.arraySize);
                SerializedProperty prop = triggers.GetArrayElementAtIndex(index);
                prop.objectReferenceValue = trigger;
            }

            obj = null;
        }
        serializedObject.ApplyModifiedProperties();
    }

}
