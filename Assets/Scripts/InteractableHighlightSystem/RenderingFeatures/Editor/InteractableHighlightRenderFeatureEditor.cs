using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableHighlightRenderFeature))]
public class InteractableHighlightRenderFeatureEditor : Editor
{
    private SerializedProperty _settings;
    private SerializedProperty _layerMask;
    private SerializedProperty _renderingLayerMask;
    private string[] _renderingLayerNames;

    private void OnEnable()
    {
        _settings = serializedObject.FindProperty("settings");
        _layerMask = _settings.FindPropertyRelative("layerMask");
        _renderingLayerMask = _settings.FindPropertyRelative("renderingLayerMask");
        _renderingLayerNames = LoadRenderingLayerNames();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_settings.FindPropertyRelative("passEvent"));
        EditorGUILayout.PropertyField(_layerMask);
        DrawRenderingLayerMaskField(_renderingLayerMask, "Rendering Layer Mask");
        EditorGUILayout.PropertyField(_settings.FindPropertyRelative("renderQueue"));
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_settings.FindPropertyRelative("overrideMaterial"));
        EditorGUILayout.PropertyField(_settings.FindPropertyRelative("overrideMaterialPassIndex"));
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_settings.FindPropertyRelative("overrideDepthState"));
        if (_settings.FindPropertyRelative("overrideDepthState").boolValue)
        {
            EditorGUILayout.PropertyField(_settings.FindPropertyRelative("depthCompare"));
            EditorGUILayout.PropertyField(_settings.FindPropertyRelative("depthWrite"));
        }

        if (serializedObject.ApplyModifiedProperties())
        {
            _renderingLayerNames = LoadRenderingLayerNames();
        }
    }

    private void DrawRenderingLayerMaskField(SerializedProperty prop, string label)
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
