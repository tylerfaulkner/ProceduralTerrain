using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinNoise))]
public class PerlinGenerationEditor : Editor
{
    SerializedProperty frequency;
    SerializedProperty amplitude;
    SerializedProperty octaves;
    SerializedProperty persistance;
    SerializedProperty useTerrainColors;
    SerializedProperty scale;
    SerializedProperty applyHeight;
    SerializedProperty autogenerate;

    SerializedProperty seed;
    SerializedProperty offsetX;
    SerializedProperty offsetZ;

    SerializedProperty colors;
    SerializedProperty heightCurve;

    SerializedProperty minMax;

    void OnEnable()
    {
        frequency = serializedObject.FindProperty("lacunarity");
        amplitude = serializedObject.FindProperty("height");
        octaves = serializedObject.FindProperty("octaves");
        persistance = serializedObject.FindProperty("persistance");
        useTerrainColors = serializedObject.FindProperty("useTerrainColors");
        scale = serializedObject.FindProperty("zoom");
        applyHeight = serializedObject.FindProperty("applyHeight");
        autogenerate = serializedObject.FindProperty("autogenerate");
        seed = serializedObject.FindProperty("seed");
        offsetX = serializedObject.FindProperty("shiftX");
        offsetZ = serializedObject.FindProperty("shiftZ");
        colors = serializedObject.FindProperty("colors");
        heightCurve = serializedObject.FindProperty("heightCurve");
        minMax = serializedObject.FindProperty("minMax");
    }


    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleLeft;
        style.richText = true;
        style.wordWrap = true;
        style.fontSize = 20;
        //style.border = GUI.skin.box.border;
        style.normal.textColor = new Color(0, 1, 0);
        style.clipping = TextClipping.Overflow;
        style.stretchWidth = true;

        GUIStyle title = new GUIStyle(style);
        title.alignment = TextAnchor.MiddleCenter;
        title.fontSize = 25;


        GUIStyle noiseStyle = new GUIStyle(style);
        noiseStyle.normal.textColor = new Color(1, 0, 0);

        GUILayout.Label("Terrain <color=cyan>Generator</color>", title);

        PerlinNoise noise = (PerlinNoise)target;

        GUILayout.Label("Noise <color=cyan>Generation</color> Properties", noiseStyle);

        serializedObject.Update();
        EditorGUILayout.PropertyField(frequency);
        EditorGUILayout.PropertyField(octaves);
        EditorGUILayout.PropertyField(persistance);
        EditorGUILayout.PropertyField(scale);
        EditorGUILayout.PropertyField(seed);
        EditorGUILayout.PropertyField(offsetX);
        EditorGUILayout.PropertyField(offsetZ);
        EditorGUILayout.PropertyField(minMax);
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(15);


        GUILayout.Label("Terrain", style, GUILayout.ExpandWidth(true));

        serializedObject.Update();
        EditorGUILayout.PropertyField(applyHeight);
        EditorGUILayout.PropertyField(amplitude);
        EditorGUILayout.PropertyField(heightCurve);
        serializedObject.ApplyModifiedProperties();

        serializedObject.Update();
        EditorGUILayout.PropertyField(useTerrainColors);
        serializedObject.ApplyModifiedProperties();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(colors);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    
        GUILayout.Space(15);


        serializedObject.Update();
        EditorGUILayout.PropertyField(autogenerate);
        serializedObject.ApplyModifiedProperties();
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();
    }
}
