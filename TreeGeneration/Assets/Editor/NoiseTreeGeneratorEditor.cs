using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseTreeGenerator))]
public class NoiseTreeGeneratorEditor : Editor
{
    float trunkMinLen = 2, trunkMinVal = 2, trunkMaxLen = 20, trunkMaxVal = 20;
    public override void OnInspectorGUI() {
        
        NoiseTreeGenerator noiseTreeGen = (NoiseTreeGenerator)target;

        EditorGUILayout.LabelField("Trunk Length Min Max", "Min: " + trunkMinLen.ToString("F2") + " Max: " + trunkMaxLen.ToString("F2"));

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.MinMaxSlider(ref trunkMinLen, ref trunkMaxLen, trunkMinVal, trunkMaxVal);
        if (EditorGUI.EndChangeCheck()) {
            // Update values for trunk
            noiseTreeGen.trunkMinLen = Mathf.Round(trunkMinLen * 100f) / 100f;
            noiseTreeGen.trunkMaxLen = Mathf.Round(trunkMaxLen * 100f) / 100f;
            if (noiseTreeGen.autoUpdate)
                noiseTreeGen.GenerateTree();
        }

        if (DrawDefaultInspector()) {
            if (noiseTreeGen.autoUpdate)
                noiseTreeGen.GenerateTree();
        }

        if (GUILayout.Button("Generate")) {
            noiseTreeGen.GenerateTree();
        }
    }
}
