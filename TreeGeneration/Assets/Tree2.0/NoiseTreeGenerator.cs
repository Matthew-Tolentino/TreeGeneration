using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTreeGenerator : MonoBehaviour
{
    [Header("Visuals")]
    public bool drawWireFrame;
    public bool autoUpdate;

    [Header("Tree Logic")]
    [HideInInspector]
    public float trunkMinLen, trunkMaxLen;
    public float branchLen = 1f;
    public int numTrunkPoints = 5;

    private Vector3 direction = Vector3.up;
    private Vector3 position;
    private Vector3[] trunkPoints;

    [Header("Trunk")]
    public int seed = 1;
    public float offsetStrength = 1;
    public Vector2 Xoffset;
    public Vector2 Yoffset;
    public Vector2 Zoffset;

    [Header("Branches")]
    public int numBrachesMin, numBrachesMax;
    public int spawnHeight;
    public int branchDepth;

    private Vector3[] branches;

    public void GenerateTree() {
        // Debug.Log(trunkMinLen + ", " + trunkMaxLen);
        GenereateTrunk();
        GenerateBranches();
    }

    public void GenereateTrunk() {
        System.Random prng = new System.Random(seed);

        // Reset position to base of trunk
        position = transform.position;
        trunkPoints = new Vector3[numTrunkPoints];
                
        // Genreate points along trunk
        trunkPoints[0] = position;
        for (int i = 1; i < numTrunkPoints; i++) {
            Vector3 offsetVec = NoiseOffset(i) * offsetStrength;

            float trunkLen = prng.Next((int)trunkMinLen, (int)trunkMaxLen);
            Debug.Log(trunkLen);

            position += (direction.normalized + offsetVec) * trunkLen;
            trunkPoints[i] = position;
            position.x = transform.position.x;
            position.z = transform.position.z;
        }
    }

    public void GenerateBranches() {
        System.Random prng = new System.Random(seed);

        int numBraches = prng.Next(numBrachesMin, numBrachesMax);

        branches = new Vector3[numBraches];

        for (int i = 0; i < numBraches; i++) {
            position = trunkPoints[prng.Next(1, numTrunkPoints)];
            // Generate rest of branches here
            branches[i] = position;
        }
    }

    Vector3 NoiseOffset(int iter) {
        System.Random prng = new System.Random(seed);
        float[] temp = new float[3];

        // Noise for X-axis offset
        float xAxisOffSetX = prng.Next(-100000, 100000) + Xoffset.x + iter;
        float xAxisOffSetY = prng.Next(-100000, 100000) + Xoffset.y + iter;
        float xPerlinValue = Mathf.PerlinNoise(xAxisOffSetX, xAxisOffSetY) * 2 - 1;

        // Noise for Y-axis offset
        float yAxisOffsetX = prng.Next(-100000, 100000) + Yoffset.x + iter;
        float yAxisOffsetY = prng.Next(-100000, 100000) + Yoffset.y + iter;
        float yPerlinValue = Mathf.PerlinNoise(yAxisOffsetX, yAxisOffsetY) * 2 - 1;

        // Noise for Z-axis offset
        float zAxisOffsetX = prng.Next(-100000, 100000) + Zoffset.x + iter;
        float zAxisOffsetY = prng.Next(-100000, 100000) + Zoffset.y + iter;
        float zPerlinVlaue = Mathf.PerlinNoise(zAxisOffsetX, zAxisOffsetY) * 2 - 1;

        return new Vector3(xPerlinValue, yPerlinValue, zPerlinVlaue);
    }

    void OnValidate() {
        if (spawnHeight > numTrunkPoints)
            spawnHeight = numTrunkPoints;
        if (spawnHeight < 0)
            spawnHeight = 0;
        if (numTrunkPoints < 2)
            numTrunkPoints = 2;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        if (drawWireFrame && trunkPoints.Length > 0) {
            // Draw Wireframe here
            for (int i = 0; i < trunkPoints.Length - 1; i++) {
                Gizmos.DrawSphere(trunkPoints[i], .1f);
                Gizmos.DrawLine(trunkPoints[i], trunkPoints[i + 1]);
            }
            Gizmos.DrawSphere(trunkPoints[trunkPoints.Length - 1], .1f);
            Gizmos.DrawLine(trunkPoints[trunkPoints.Length - 2], trunkPoints[trunkPoints.Length - 1]);

            Gizmos.color = Color.red;
            for (int j = 0; j < branches.Length; j++) {
                Gizmos.DrawWireSphere(branches[j], .2f);
            }
        }
    }

    void printArray(Vector3[] arr) {
        string str = "[";
        foreach (Vector3 v in arr) {
            str += v + ", ";
        }
        str += "]";
        Debug.Log(str);
    }
}
