using UnityEditorInternal;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(GlobalObjects))]
public class GlobalObjectsEditor : Editor {

    private ReorderableList reorderableList;

    public GlobalObjects Instance { get { return (GlobalObjects)target; } }

    [MenuItem("Assets/Create/Editor/Global Objects Manager")]
    private static void CreateManager()
    {
        GlobalObjects manager = ScriptableObject.CreateInstance<GlobalObjects>();

        Utility.EnsurePathExists("Resources");

        AssetDatabase.CreateAsset(manager, "Assets/Resources/Global Objects.asset");
    }
    private void CreateList()
    {
        reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("references"), true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Global Objects");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            GlobalObjects.ObjectReference reference = Instance.GetReference(index);

            reference.Key = EditorGUI.TextField(
                new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                "Key",
                reference.Key);

            reference.Object = EditorGUI.ObjectField(
                new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                "Object",
                reference.Object,
                typeof(Object),
                false);

            Instance.SetReference(index, reference);
        };
    }
    public override void OnInspectorGUI()
    {
        if (reorderableList == null)
            CreateList();

        EditorGUIUtility.labelWidth = 50;

        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(target);
    }
}
