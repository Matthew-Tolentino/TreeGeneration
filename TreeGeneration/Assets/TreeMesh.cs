using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TreeMesh : MonoBehaviour
{
    public float radius = 1f;
    public Vector3 direction;
    public float angle = 60;
    public float length = 1f;

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector3 tan;
    private Vector3 binormal;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateCircleVerts(transform.position, radius, direction);
        //GenerateCircleVerts(transform.position, radius, Vector3.up);

        //CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0)
        };

        triangles = new int[]
        {
            0, 1, 2
        };
    }

    void GenerateCircleVerts(Vector3 center, float radius, Vector3 direction)
    {
        Vector3[] v = new Vector3[12];

        tan = Vector3.zero;
        binormal = Vector3.zero;

        Vector3.OrthoNormalize(ref direction, ref tan, ref binormal);

        for (int i = 0; i < v.Length/2; i++)
        {
            Vector3 rotatedVector = RotateVector(direction, tan, binormal, i);
            v[i] = (center + rotatedVector) * radius;
            v[i + 6] = (((center + rotatedVector) * radius) + (direction * length));
        }

        int[] t = new int[3 * v.Length];

        for (int j = 0; j < t.Length; j += 6)
        {
            if (j < t.Length - 6)
            {
                t[j] = j / 6;
                t[j + 1] = j / 6 + 1;
                t[j + 2] = j / 6 + 6;
                t[j + 3] = j / 6 + 6;
                t[j + 4] = j / 6 + 1;
                t[j + 5] = j / 6 + 7;
            }
            else
            {
                t[j] = j / 6;
                t[j + 1] = 0;
                t[j + 2] = j / 6 + 6;
                t[j + 3] = j / 6 + 6;
                t[j + 4] = 0;
                t[j + 5] = 6;
            }
        }



        vertices = v;
        triangles = t;
    }

    Vector3 RotateVector(Vector3 direction, Vector3 tan, Vector3 binormal, int iter)
    {
        float tanM = tan.magnitude;
        float biM = binormal.magnitude;
        Vector3 newV = tanM * (((Mathf.Cos(angle * iter) / tanM) * tan) + ((Mathf.Sin(angle * iter) / biM) * binormal));
        return newV;
    }

    void OnDrawGizmos()
    {
        GenerateCircleVerts(transform.position, radius, direction);
        Gizmos.DrawLine(Vector3.zero, direction * length);
        Gizmos.DrawLine(Vector3.zero, tan);
        Gizmos.DrawLine(Vector3.zero, binormal);

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

    void UpdateMesh()
    {
        string s = "[";
        foreach (int i in triangles)
        {
            s += i + ", ";
        }
        Debug.Log(s + "]");
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
