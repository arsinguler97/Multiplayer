using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableHighlight))]
public class InteractableHighlightEditor : Editor
{
    private SerializedProperty _interactableMask;
    private SerializedProperty _inUseMask;
    private SerializedProperty _defaultMask;
    private SerializedProperty _setDefaultOnEnable;
    private string[] _renderingLayerNames;

    private void OnEnable()
    {
        _interactableMask = serializedObject.FindProperty("interactableMask");
        _inUseMask = serializedObject.FindProperty("inUseMask");
        _defaultMask = serializedObject.FindProperty("defaultMask");
        _setDefaultOnEnable = serializedObject.FindProperty("setDefaultOnEnable");
        _renderingLayerNames = LoadRenderingLayerNames();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawMaskField(_interactableMask, "Interactable Mask");
        DrawMaskField(_inUseMask, "In Use Mask");
        DrawMaskField(_defaultMask, "Default Mask");
        EditorGUILayout.PropertyField(_setDefaultOnEnable);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("includeInactive"));

        if (serializedObject.ApplyModifiedProperties())
        {
            _renderingLayerNames = LoadRenderingLayerNames();
        }
    }

    private void DrawMaskField(SerializedProperty prop, string label)
    {
        int current = (int)prop.longValue;
        int next = EditorGUILayout.MaskField(label, current, _renderingLayerNames);
        if (next != current)
        {
            prop.longValue = (uint)next;
        }
    }

    private static string[] LoadRenderingLayerNames()
    {
        string[] fallback = BuildFallbackNames();
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (assets == null || assets.Length == 0)
            return fallback;

        SerializedObject tagManager = new SerializedObject(assets[0]);
        SerializedProperty layersProp = tagManager.FindProperty("m_RenderingLayers");
        if (layersProp == null || !layersProp.isArray)
            return fallback;

        int count = layersProp.arraySize;
        if (count <= 0)
            return fallback;

        List<string> names = new List<string>(count);
        for (int i = 0; i < count; i++)
        {
            SerializedProperty element = layersProp.GetArrayElementAtIndex(i);
            string name = element != null ? element.stringValue : string.Empty;
            if (string.IsNullOrEmpty(name))
                name = "Layer " + i;
            names.Add(name);
        }

        return names.ToArray();
    }

    private static string[] BuildFallbackNames()
    {
        string[] names = new string[32];
        for (int i = 0; i < names.Length; i++)
            names[i] = "Layer " + i;
        return names;
    }
}
