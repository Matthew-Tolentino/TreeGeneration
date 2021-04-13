using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float len;
    public int verts = 6;
    public float radius = 1f;
    // public float angle = 1f;
    Vector3 dir = Vector3.up;
    Vector3 tan, binormal = Vector3.zero;
    // Start is called before the first frame update
    Vector3[] vertices;
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.V)) {
            Vector3.OrthoNormalize(ref dir, ref tan, ref binormal);
        }
    }

    void GenerateVerts() {
        vertices = new Vector3[verts];
        Vector3.OrthoNormalize(ref dir, ref tan, ref binormal);
        for (int i = 0; i < verts; i++) {
            vertices[i] = RotateVector(dir, tan, binormal, i) * radius;
        }
    }

    Vector3 RotateVector(Vector3 direction, Vector3 tan, Vector3 binormal, int iter)
    {
        float angle = 360f / (float)verts;
        Debug.Log(angle);
        float trueAngle = angle * Mathf.PI/180f;
        float tanM = tan.magnitude;
        float biM = binormal.magnitude;
        Vector3 newV = tanM * ((Mathf.Cos(trueAngle * iter) / tanM * tan) + (Mathf.Sin(trueAngle * iter) / biM * binormal));
        //Vector3 newV = tanM * (((Mathf.Cos(angle * iter) / tanM) * tan) + ((Mathf.Sin(angle * iter) / biM) * binormal));
        return newV;
    }

    void OnDrawGizmos() {
        GenerateVerts();

        Debug.Log(dir);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, dir);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, tan);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, binormal);

        Gizmos.color = Color.white;
        for (int i = 0; i < verts; i++) {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
