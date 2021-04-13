using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTreeMeshGenerator
{
    public static NoiseTreeMeshData GenerateMesh(TrunkPoint[] trunkPoints, List<Branch> branches, int detailVerts, float trunkRadius = 1f, float branchRadius = 1f, float thinningRate = 100f) {
        Dictionary<Vector3, int[]> pointToVerts = new Dictionary<Vector3, int[]>();
        
        int segments = trunkPoints.Length - 1;
        for (int q = 0; q < branches.Count; q++) {
            segments += branches[q].segmentCount;
        }
        
        NoiseTreeMeshData meshData = new NoiseTreeMeshData(segments, detailVerts);
        int vertexIndex = 0;

        // Calculate verts for trunk thickness
        for (int i = 0; i < trunkPoints.Length; i++) {
            int[] verticesToStore = new int[detailVerts];
            Vector3 dir = trunkPoints[i].dir;
            Vector3 tan = Vector3.zero, binormal = Vector3.zero;

            Vector3.OrthoNormalize(ref dir, ref tan, ref binormal);

            for (int j = 0; j < detailVerts; j++) {
                if (i == trunkPoints.Length - 1)
                    meshData.vertices[vertexIndex] = trunkPoints[i].pos;
                else { // Collapse verts if its an end point
                    Vector3 rotatedVector = RotateVector(dir, tan, binormal, j, detailVerts);
                    meshData.vertices[vertexIndex] = trunkPoints[i].pos + rotatedVector * trunkRadius;
                }

                verticesToStore[j] = vertexIndex++;
            }

            pointToVerts.Add(trunkPoints[i].pos, verticesToStore);

            // Generate Trianlges for trunk
            if (i > 0) { // Check to see if past first iteration
                int[] pt1Verts = pointToVerts[trunkPoints[i - 1].pos];
                for (int w = 0; w < detailVerts; w++) {
                    if (w < detailVerts - 1) {
                        meshData.AddTriangle(pt1Verts[w], pt1Verts[w + 1], vertexIndex - detailVerts + w);
                        meshData.AddTriangle(vertexIndex - detailVerts + w, pt1Verts[w + 1], vertexIndex - detailVerts + 1 + w);
                    } else {
                        meshData.AddTriangle(pt1Verts[detailVerts - 1], pt1Verts[0], vertexIndex - 1);
                        meshData.AddTriangle(vertexIndex - 1, pt1Verts[0], vertexIndex - detailVerts);
                    }
                }
            }
        }

        // Calculate verts for branch thickness
        for (int k = 0; k < branches.Count; k++) {
            GenerateVertPoints(branches[k], meshData, pointToVerts, detailVerts, branchRadius, thinningRate, ref vertexIndex);
        }

        return meshData;
    }

    static void GenerateVertPoints(Branch branch, NoiseTreeMeshData meshData, Dictionary<Vector3, int[]> pointToVerts, int detailVerts, float branchRadius, float thinningRate, ref int vertexIndex) {
        float branchRad = branchRadius;
        
        // Loop through points in the branch
        for (int i = 1; i < branch.points.Count; i++) {
            int[] verticesToStore = new int[detailVerts];

            Vector3 dir = branch.points[i] - branch.points[i - 1];
            Vector3 tan = Vector3.zero, binormal = Vector3.zero;
            Vector3.OrthoNormalize(ref dir, ref tan, ref binormal);

            branchRad *= thinningRate / 100f;

            // Generate verts for branch thickness at point i
            for (int j = 0; j < detailVerts; j++) {
                if (i == branch.points.Count - 1)
                    meshData.vertices[vertexIndex] = branch.points[i];
                else {
                    Vector3 rotatedVector = RotateVector(dir, tan, binormal, j, detailVerts);
                    meshData.vertices[vertexIndex] = branch.points[i] + rotatedVector * branchRad;
                }

                verticesToStore[j] = vertexIndex++;
            }
            
            pointToVerts.Add(branch.points[i], verticesToStore);

            // See if point splits and generate split branch
            if (branch.branches.Count > 0) {
                for (int k = 0; k < branch.branches.Count; k++) {
                    if (branch.branches[k].points[0] == branch.points[i]) {
                        GenerateVertPoints(branch.branches[k], meshData, pointToVerts, detailVerts, branchRad * thinningRate / 100f, thinningRate, ref vertexIndex);
                        break;
                    }
                }
            }
        }

        // Figuring out how to skillfully insert triangle generation in the loop to make branch is too hard so im just doing it after
        // Will be redundant but will work

        // Generate triangles for branch
        for (int k = 1; k < branch.points.Count; k++) {
            int[] pt1Verts = pointToVerts[branch.points[k - 1]];
            int[] pt2Verts = pointToVerts[branch.points[k]];
            for (int w = 0; w < detailVerts; w++) {
                if (w < detailVerts - 1) {
                    meshData.AddTriangle(pt1Verts[w], pt1Verts[w + 1], pt2Verts[w]);
                    meshData.AddTriangle(pt2Verts[w], pt1Verts[w + 1], pt2Verts[w + 1]);
                } else {
                    meshData.AddTriangle(pt1Verts[detailVerts - 1], pt1Verts[0], pt2Verts[detailVerts - 1]);
                    meshData.AddTriangle(pt2Verts[detailVerts - 1], pt1Verts[0], pt2Verts[0]);
                }
            }
        }
    }

    static Vector3 RotateVector(Vector3 dir, Vector3 tan, Vector3 binormal, int iter, int detailVerts) {
        float tanM = tan.magnitude, biM = binormal.magnitude;
        float angle = 360f / (float)detailVerts * Mathf.PI / 180f;
        Vector3 rotatedVector = tanM * ((Mathf.Cos(angle * iter) / tanM * tan) + (Mathf.Sin(angle * iter) / biM * binormal));
        return rotatedVector;
    }
}

public class NoiseTreeMeshData{
    public Vector3[] vertices;
    public int[] triangles;

    int triangleIndex;

    // segments: number of lines that make up the tree
    // detailVerts: number of verts surrounding points
    public NoiseTreeMeshData(int segments, int detailVerts) {
        vertices = new Vector3[(segments + 1) * detailVerts];

        /* 6 gotten from 2 * 3
         * 2: upper and lower rings used to make triangles
         * 3: number of verts needed to make a triangle
         */
        triangles = new int[segments * detailVerts * 6];
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}