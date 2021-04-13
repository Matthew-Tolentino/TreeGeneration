using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTreeGenerator : MonoBehaviour
{
    [Header("Visuals")]
    public bool drawWireFrame;
    public bool drawWireFrameVerts;
    public bool drawMesh;
    public bool autoUpdate;

    [Header("Tree Logic")]
    [HideInInspector]
    public float trunkMinLen, trunkMaxLen;
    public int seed = 1;

    [Header("Trunk")]
    public int numTrunkPoints = 5;
    public float offsetStrength = 1;
    [Range(3, 12)]
    public int detailVerts = 6;
    [Range(.1f, 5f)]
    public float trunkRadius = 1f;
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
    [Range(1, 5)]
    public int branchDepth;
    [Range(1, 180)]
    public float branchAngle;
    [Range(0, 100)]
    public float splitChance;
    [Range(.1f, 5f)]
    public float branchRadius = 1f;
    [Range(1f, 100f)]
    [Tooltip("Rate at which branches thin based off last point. (ex: 90% of last point)")]
    public float thinningRate = 90;

    [Header("Mesh Generation")]
    public MeshFilter meshFilter;

    private Vector3 position;
    private TrunkPoint[] trunkPoints;

    private List<Branch> branches;
    private NoiseTreeMeshData meshData;

    public void GenerateTree() {
        // Debug.Log(trunkMinLen + ", " + trunkMaxLen);
        GenereateTrunk();
        GenerateBranches();
        GenerateMesh();
    }

    public void GenereateTrunk() {
        System.Random prng = new System.Random(seed);
        Vector3 direction = Vector3.up;

        // Reset position to base of trunk
        position = transform.position;
        trunkPoints = new TrunkPoint[numTrunkPoints];
                
        // Genreate points along trunk
        for (int i = 0; i < numTrunkPoints; i++) {
            Vector3 offsetVec = NoiseOffset(i) * offsetStrength;

            float trunkLen = prng.Next((int)trunkMinLen, (int)trunkMaxLen);

            trunkPoints[i].pos = position;
            trunkPoints[i].dir = direction;

            // Replace Vector3.up with direction to make trunk bendy
            direction = (Vector3.up + offsetVec).normalized;
            position += direction * trunkLen;
        }

        // trunkPoints[0].pos = position;
        // for (int i = 1; i < numTrunkPoints; i++) {
        //     Vector3 offsetVec = NoiseOffset(i) * offsetStrength;

        //     float trunkLen = prng.Next((int)trunkMinLen, (int)trunkMaxLen);

        //     position += (direction.normalized + offsetVec) * trunkLen;
        //     trunkPoints[i].pos = position;
        //     position.x = transform.position.x;
        //     position.z = transform.position.z;
        // }
    }

    public void GenerateBranches() {
        System.Random prng = new System.Random(seed);

        int numBraches = prng.Next(numBrachesMin, numBrachesMax);

        branches = new List<Branch>();

        for (int i = 0; i < numBraches; i++) {
            position = trunkPoints[prng.Next(spawnHeight, numTrunkPoints - 1)].pos;

            // Generate starting point on branches
            Vector3 direction = RotateDirection(branchAngle, Vector3.up, prng, 100f);
            Branch newBranch = GenerateBranch(direction, position, branchAngle, branchDepth, splitChance, prng);

            branches.Add(newBranch);
        }
    }

    // Recursive function that makes branches and all sub-branches
    public Branch GenerateBranch(Vector3 dir, Vector3 startPos, float changeAngle, int depth, float splitChance, System.Random prng) {
        if (depth <= 0)
            return null; 

        Branch newBranch = new Branch();
        float newBranchPoints = prng.Next(branchPointsMin, branchPointsMax);
        float newBranchLen = prng.Next((int)branchMinLen, (int)branchMaxLen);
        newBranch.segmentCount = (int)newBranchPoints - 1;
        Vector3 position = startPos;

        // Generate points for branch
        newBranch.points.Add(startPos);
        for (int i = 1; i < newBranchPoints; i++) {
            Vector3 newDir = RotateDirection(changeAngle, dir, prng, 100f);
            position += newDir * newBranchLen;
            newBranch.points.Add(position);
            float split = prng.Next(0, 100);
            // Chance to make sub-branch
            if (split < splitChance && i != newBranchPoints - 1) {
                Branch splitBranch = GenerateBranch(newDir, position, changeAngle, depth - 1, splitChance, prng);
                if (splitBranch != null) {
                    newBranch.segmentCount += splitBranch.segmentCount;
                    newBranch.branches.Add(splitBranch);
                }
            }
        }

        return newBranch;
    }

    public void GenerateMesh() {
        if (!drawMesh) {
            meshFilter.sharedMesh = new Mesh();
            return;
        }

        meshData = NoiseTreeMeshGenerator.GenerateMesh(trunkPoints, branches, detailVerts, trunkRadius, branchRadius, thinningRate);
        meshFilter.sharedMesh = meshData.CreateMesh();
    }

    public void ResetNoiseTree() {
        trunkPoints = new TrunkPoint[numTrunkPoints];
        branches = new List<Branch>();
        meshData = null;
    }

    public Vector3 RotateDirection(float angle, Vector3 dir, System.Random prng, float rotThresh) {
        Vector3 newDir = dir;
        // rotate only on x or z if direction is up
        if (newDir == Vector3.up) {
            float rotPick = prng.Next(0, 4);
            if (rotPick == 0)
                newDir = Quaternion.Euler(angle, 0, 0) * newDir;
            else if (rotPick == 1)
                newDir = Quaternion.Euler(-angle, 0, 0) * newDir;
            else if (rotPick == 2)
                newDir = Quaternion.Euler(0, 0, angle) * newDir;
            else
                newDir = Quaternion.Euler(0, 0, -angle) * newDir;
        }

        float rotChance = prng.Next(0, 100);
        
        if (rotChance < rotThresh) {
            float rotPick = prng.Next(0, 6);
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
                // Draw Trunk Wireframe
                for (int i = 0; i < trunkPoints.Length - 1; i++) {
                    Gizmos.DrawSphere(trunkPoints[i].pos, .1f);
                    Gizmos.DrawLine(trunkPoints[i].pos, trunkPoints[i + 1].pos);
                }

                Gizmos.color = Color.red;
                // Draw Branches Wireframe
                for (int j = 0; j < branches.Count; j++) {
                    GizmosDrawBranch(branches[j]);
                }

                if (drawWireFrameVerts) {
                    Gizmos.color = Color.white;
                    for (int k = 0; k < meshData.vertices.Length; k++) {
                        Gizmos.DrawSphere(meshData.vertices[k], .1f);
                    }
                }
            }
        } catch {
            GenerateTree();
        }
    }

    void GizmosDrawBranch(Branch branch) {
        // Draw Lines and points for current branch
        for (int i = 0; i < branch.points.Count - 1; i++) {
            Gizmos.DrawWireSphere(branch.points[i], .1f);
            Gizmos.DrawLine(branch.points[i], branch.points[i + 1]);
        }

        // If there are sub-branches draw them
        if (branch.branches.Count > 0) {
            for (int j = 0; j < branch.branches.Count; j++) {
                // Debug.Log(branch.branches[j].points[0]);
                // Debug.Log(branch.branches[j].points[1]);
                GizmosDrawBranch(branch.branches[j]);
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

public struct TrunkPoint {
    public Vector3 pos;
    public Vector3 dir;

    public TrunkPoint(Vector3 _pos, Vector3 _dir) {
        this.pos = _pos;
        this.dir = _dir;
    }
}
