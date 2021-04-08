using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch
{
    public List<Vector3> points;
    public List<Branch> branches;

    public Branch() {
        points = new List<Vector3>();
        branches = new List<Branch>();
    }
}
