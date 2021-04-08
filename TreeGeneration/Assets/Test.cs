using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float len;
    Vector3 dir = Vector3.up;
    // Start is called before the first frame update
    void Start()
    {
        
        
        Debug.Log(dir);
    }

    void OnDrawGizmos() {
        dir = Quaternion.Euler(30f, 0, 0) * dir;
        Gizmos.DrawLine(Vector3.zero, dir * len);
    }
}
