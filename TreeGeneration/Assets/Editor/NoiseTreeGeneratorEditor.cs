using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseTreeGenerator))]
public class NoiseTreeGeneratorEditor : Editor
{
    float trunkMinLen = 2, trunkMinVal = 2, trunkMaxLen = 4, trunkMaxVal = 20;
    float branchMinNum = 0, branchMinVal = 0, branchMaxNum = 4, branchMaxVal = 10;
    float branchMinLen = 1, branchMinLenVal = 1, branchMaxLen = 4, branchMaxLenVal = 10;
    float branchPointsMinNum = 2, branchPointsMinVal = 2, branchPointsMaxNum = 5, branchPointsMaxVal = 10;

    public override void OnInspectorGUI() {
        
        NoiseTreeGenerator noiseTreeGen = (NoiseTreeGenerator)target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Trunk Length Min Max", "Min: " + trunkMinLen.ToString("F2") + " Max: " + trunkMaxLen.ToString("F2"));
        EditorGUILayout.MinMaxSlider(ref trunkMinLen, ref trunkMaxLen, trunkMinVal, trunkMaxVal);

        EditorGUILayout.LabelField("Num Branches Min Max", "Min: " + branchMinNum.ToString("F0") + " Max: " + branchMaxNum.ToString("F0"));
        EditorGUILayout.MinMaxSlider(ref branchMinNum, ref branchMaxNum, branchMinVal, branchMaxVal);

        EditorGUILayout.LabelField("Branch Len Min Max", "Min: " + branchMinLen.ToString("F2") + " Max: " + branchMaxLen.ToString("F2"));
        EditorGUILayout.MinMaxSlider(ref branchMinLen, ref branchMaxLen, branchMinLenVal, branchMaxLenVal);

        EditorGUILayout.LabelField("Branch Points Min Max", "Min: " + branchPointsMinNum.ToString("F0") + " Max: " + branchPointsMaxNum.ToString("F0"));
        EditorGUILayout.MinMaxSlider(ref branchPointsMinNum, ref branchPointsMaxNum, branchPointsMinVal, branchPointsMaxVal);
        if (EditorGUI.EndChangeCheck()) {
            // Update values for trunk
            noiseTreeGen.trunkMinLen = Mathf.Round(trunkMinLen * 100f) / 100f;
            noiseTreeGen.trunkMaxLen = Mathf.Round(trunkMaxLen * 100f) / 100f;
            // Update values for branches
            noiseTreeGen.numBrachesMin = (int)Mathf.Round(branchMinNum);
            noiseTreeGen.numBrachesMax = (int)Mathf.Round(branchMaxNum);
            noiseTreeGen.branchMinLen = Mathf.Round(branchMinLen * 100) / 100f;
            noiseTreeGen.branchMaxLen = Mathf.Round(branchMaxLen * 100) / 100f;
            noiseTreeGen.branchPointsMin = (int)Mathf.Round(branchPointsMinNum);
            noiseTreeGen.branchPointsMax = (int)Mathf.Round(branchPointsMaxNum);
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
