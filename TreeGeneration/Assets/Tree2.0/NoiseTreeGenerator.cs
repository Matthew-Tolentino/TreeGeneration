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
    [HideInInspector]
    public int numBrachesMin, numBrachesMax;
    [HideInInspector]
    public float branchMinLen, branchMaxLen;
    [HideInInspector]
    public int branchPointsMin, branchPointsMax;
    public int spawnHeight;
    public int branchDepth;
    [Range(1, 60)]
    public float branchAngle;
    [Range(0, 100)]
    public float splitChance;

    private Vector3[] branchPoints;
    private List<Branch> branches;

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

            position += (direction.normalized + offsetVec) * trunkLen;
            trunkPoints[i] = position;
            position.x = transform.position.x;
            position.z = transform.position.z;
        }
    }

    public void GenerateBranches() {
        System.Random prng = new System.Random(seed);

        int numBraches = prng.Next(numBrachesMin, numBrachesMax);

        // branchPoints = new Vector3[numBraches];
        branches = new List<Branch>();

        for (int i = 0; i < numBraches; i++) {
            position = trunkPoints[prng.Next(spawnHeight, numTrunkPoints)];
            // Generate starting point on branches
            Branch new_Branch = new Branch();
            new_Branch.points.Add(position);

            Vector3 direction = RotateDirection(branchAngle, Vector3.up, prng, 100f);
            Branch newBranch = GenerateBranch(direction, position, branchAngle, branchDepth, splitChance, prng);

            branches.Add(newBranch);
        }
    }

    public Branch GenerateBranch(Vector3 dir, Vector3 startPos, float changeAngle, int depth, float splitChance, System.Random prng) {
        Branch newBranch = new Branch();
        float newBranchPoints = prng.Next(branchPointsMin, branchPointsMax);
        float newBranchLen = prng.Next((int)branchMinLen, (int)branchMaxLen);
        Vector3 position = startPos;

        // Generate points for branch
        newBranch.points.Add(startPos);
        for (int i = 1; i < newBranchPoints; i++) {
            Vector3 newDir = RotateDirection(changeAngle, dir, prng, 100f);
            position += newDir * newBranchLen;
            newBranch.points.Add(position);
            // Debug.Log(position);
        }

        return newBranch;
    }

    public Vector3 RotateDirection(float angle, Vector3 dir, System.Random prng, float rotThresh) {
        Vector3 newDir = dir;
        // rotate only on x or z if direction is up
        if (newDir == Vector3.up) {
            Debug.Log("Up");
            float rotPick = prng.Next(0, 4);
            Debug.Log(rotPick);
            if (rotPick == 0)
                newDir = Quaternion.Euler(angle, 0, 0) * newDir;
            else if (rotPick == 1)
                newDir = Quaternion.Euler(-angle, 0, 0) * newDir;
            else if (rotPick == 2)
                newDir = Quaternion.Euler(0, 0, angle) * newDir;
            else
                newDir = Quaternion.Euler(0, 0, -angle) * newDir;
            Debug.Log(newDir);
        }

        float rotChance = prng.Next(0, 100);
        
        if (rotChance < rotThresh) {
            Debug.Log("rotating");
            float rotPick = prng.Next(0, 6);
            Debug.Log(rotPick);
            if (rotPick == 0)
                newDir = Quaternion.Euler(angle, 0, 0) * newDir;
            else if (rotPick == 1)
                newDir = Quaternion.Euler(-angle, 0, 0) * newDir;
            else if (rotPick == 2)
                newDir = Quaternion.Euler(0, 0, angle) * newDir;
            else if (rotPick == 3)
                newDir = Quaternion.Euler(0, 0, -angle) * newDir;
            else if (rotPick == 4)
                newDir = Quaternion.Euler(0, angle, 0) * newDir;
            else
                newDir = Quaternion.Euler(0, -angle, 0) * newDir;
        }

        Debug.Log(dir);
        Debug.Log(newDir);
        
        return newDir;
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
        try {
            if (drawWireFrame && trunkPoints.Length > 0) {
                // Draw Wireframe here
                for (int i = 0; i < trunkPoints.Length - 1; i++) {
                    Gizmos.DrawSphere(trunkPoints[i], .1f);
                    Gizmos.DrawLine(trunkPoints[i], trunkPoints[i + 1]);
                }
                Gizmos.DrawSphere(trunkPoints[trunkPoints.Length - 1], .1f);
                Gizmos.DrawLine(trunkPoints[trunkPoints.Length - 2], trunkPoints[trunkPoints.Length - 1]);

                Gizmos.color = Color.red;
                for (int j = 0; j < branches.Count; j++) {
                    for (int k = 0; k < branches[j].points.Count - 1; k++) {
                        Gizmos.DrawWireSphere(branches[j].points[k], .1f);
                        Gizmos.DrawLine(branches[j].points[k], branches[j].points[k + 1]);
                    }
                    Gizmos.DrawWireSphere(branches[j].points[branches[j].points.Count - 1], .1f);
                    Gizmos.DrawLine(branches[j].points[branches[j].points.Count - 2], branches[j].points[branches[j].points.Count - 1]);
                }
            }
        } catch {
            GenerateTree();
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
