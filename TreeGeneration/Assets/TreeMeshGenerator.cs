using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TreeMeshGenerator
{
    public static MeshData GenerateTreeMesh(List<Line> branches, float angle = 1.05f, float radius = 1f)
    {
        MeshData meshData = new MeshData(branches.Count);
        int vertexIndex = 0;
        List<Vector3> visitedPoints = new List<Vector3>();
        Dictionary<Vector3, int[]> pointVerts = new Dictionary<Vector3, int[]>();
        List<Vector3> closePoints = FindClosePoints(branches);

        for (int i = 0; i < branches.Count; i++)
        {
            Vector3 direction = branches[i].pos2 - branches[i].pos1;
            Vector3 tan = Vector3.zero;
            Vector3 binormal = Vector3.zero;

            Vector3.OrthoNormalize(ref direction, ref tan, ref binormal);

            // 6 is the static number of verticies used
            // Would like to make it a variable later
            if (!visitedPoints.Contains(branches[i].pos1))
            {
                int[] verticesToStore = new int[6];
                for (int j = 0; j < 6; j++)
                {
                    Vector3 rotatedVector = RotateVector(direction, tan, binormal, j, angle);

                    if (!closePoints.Contains(branches[i].pos1))
                        meshData.vertices[vertexIndex] = (branches[i].pos1 + rotatedVector) * radius;
                    else
                        meshData.vertices[vertexIndex] = branches[i].pos1;

                    verticesToStore[j] = vertexIndex;
                    vertexIndex++;
                }
                // Store vertex indices associated with point
                pointVerts.Add(branches[i].pos1, verticesToStore);
                visitedPoints.Add(branches[i].pos1);
            }

            if (!visitedPoints.Contains(branches[i].pos2))
            {
                int[] verticesToStore = new int[6];
                for (int j = 0; j < 6; j++)
                {
                    Vector3 rotatedVector = RotateVector(direction, tan, binormal, j, angle);

                    if (!closePoints.Contains(branches[i].pos2))
                        meshData.vertices[vertexIndex] = (branches[i].pos2 + rotatedVector) * radius;
                    else
                        meshData.vertices[vertexIndex] = branches[i].pos2;

                    verticesToStore[j] = vertexIndex;
                    vertexIndex++;
                }

                // Store vertex indices associated with point
                pointVerts.Add(branches[i].pos2, verticesToStore);
                visitedPoints.Add(branches[i].pos2);

                // Triangle Generation
                for (int k = 0; k < 6; k++)
                {
                    int[] pt1Verts = pointVerts[branches[i].pos1];

                    if (k < 5)
                    {
                        meshData.AddTriangle(pt1Verts[k], pt1Verts[k + 1], vertexIndex - 6 + k);
                        meshData.AddTriangle(vertexIndex - 6 + k, pt1Verts[k + 1], vertexIndex - 5 + k);
                    }
                    else
                    {
                        meshData.AddTriangle(pt1Verts[pt1Verts.Length - 1], pt1Verts[0], vertexIndex - 1);
                        meshData.AddTriangle(vertexIndex - 1, pt1Verts[0], vertexIndex - 6);
                    }
                }
            }
        }
        return meshData;
    }

    static Vector3 RotateVector(Vector3 direction, Vector3 tan, Vector3 binormal, int iter, float angle)
    {
        float tanM = tan.magnitude;
        float biM = binormal.magnitude;
        Vector3 newV = tanM * (((Mathf.Cos(angle * iter) / tanM) * tan) + ((Mathf.Sin(angle * iter) / biM) * binormal));
        return newV;
    }

    static List<Vector3> FindClosePoints(List<Line> branches)
    {
        Dictionary<Vector3, int> occurances = new Dictionary<Vector3, int>();
        foreach (Line l in branches)
        {
            try {
                occurances[l.pos1] += 1;
            } catch (KeyNotFoundException) {
                occurances[l.pos1] = 1;
            }

            try {
                occurances[l.pos2] += 1;
            } catch (KeyNotFoundException) {
                occurances[l.pos2] = 1;
            }
        }

        List<Vector3> closedPoints = new List<Vector3>();
        foreach (Vector3 key in occurances.Keys)
        {
            if (occurances[key] == 1)
                closedPoints.Add(key);
        }

        // Dont collapse base
        if (closedPoints.Count > 0)
            closedPoints.RemoveAt(0);

        return closedPoints;
    }
}

public class MeshData{
    public Vector3[] vertices;
    public int[] triangles;

    int triangleIndex;

    public MeshData(int branches)
    {
        // 6 is the static number of verticies used
        // Would like to make it a variable later
        vertices = new Vector3[branches * 6 + 6];
        triangles = new int[branches * 12 * 3];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}

