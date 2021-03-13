using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TreeGenerator treeGen = (TreeGenerator)target;

        if (DrawDefaultInspector())
        {
            if (treeGen.autoUpdate)
            {
                treeGen.GenerateTreeMesh();
            }

        }

        if (GUILayout.Button("Generate"))
        {
            treeGen.GenerateTree();
        }

        if (GUILayout.Button("Generate Next Iteration"))
        {
            treeGen.GenerateNextIteration();
        }

        if (GUILayout.Button("Reset"))
        {
            treeGen.Reset();
        }
    }
}
