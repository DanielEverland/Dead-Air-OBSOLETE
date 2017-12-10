using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(GenericObjectPoolObject))]
public class GenericObjectPoolObjectEditor : Editor {

    private ReorderableList reorderableList;

    public GenericObjectPoolObject Instance { get { return (GenericObjectPoolObject)target; } }

    private const float PADDING_TOP = 5;
    private const float PADDING_BOTTOM = 5;
    private const float ELEMENT_SPACING = 5;
    private const float ROWS_PER_ELEMENT = 2;
    
    private const float LIST_SPACING = 5;

    private void CreateList()
    {
        reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("references"), true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Object References");
        };

        reorderableList.elementHeight = (EditorGUIUtility.singleLineHeight * ROWS_PER_ELEMENT) + PADDING_TOP + PADDING_BOTTOM + (ROWS_PER_ELEMENT - 1) * ELEMENT_SPACING;

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            GenericObjectPoolObject.ObjectReference reference = Instance.GetReference(index);

            float y = rect.y;

            y += PADDING_TOP;

            reference.Key = EditorGUI.TextField(
                new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                "Key",
                reference.Key);

            y += ELEMENT_SPACING + EditorGUIUtility.singleLineHeight;

            reference.Object = EditorGUI.ObjectField(
                new Rect(rect.x, y, rect.width / 2 - LIST_SPACING, EditorGUIUtility.singleLineHeight),
                "Object",
                reference.Object,
                typeof(Object),
                false);

            reference.Instances = EditorGUI.IntSlider(
                new Rect(rect.x + rect.width / 2, y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                "Instances",
                reference.Instances,
                0,
                100);

            Instance.SetReference(index, reference);
        };
    }
    public override void OnInspectorGUI()
    {
        if (reorderableList == null)
            CreateList();

        EditorGUIUtility.labelWidth = 80;

        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(target);
    }
}
