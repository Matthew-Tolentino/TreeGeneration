using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [Header("Visuals")]
    public bool drawWireFrame;
    public bool autoUpdate;
    public bool drawMesh;

    [Header("Tree Logic")]
    public float branchLen = 1f;
    public int angle = 30;
    [Range(.5f,5f)]
    public float radius = 1f;
    [Range(.99f, 1.05f)]
    public float thinning = 1f;
    [Range(.99f, 1.05f)]
    public float trimming = 1f;
    public string axiom;
    public string sentence;
    public Rule[] rules;

    [Header("Mesh Generation")]
    public MeshFilter meshFilter;

    private Vector3 origin;
    private Vector3 position;
    private Vector3 direction = new Vector3(0, 1, 0);
    private Stack point = new Stack();
    [HideInInspector]
    public List<Line> lines = new List<Line>();

    // Temp
    MeshData m = null;

    public void GenerateTree()
    {
        sentence = axiom;
        origin = transform.position;
        position = origin;

        GenerateTreeLSystem();
        GenerateTreeMesh();
    }

    public void GenerateNextIteration()
    {
        GenerateSentence();
        lines.Clear();
        position = origin;
        GenerateTreeLSystem();
        GenerateTreeMesh();
    }

    public void GenerateTreeMesh()
    {
        if (drawMesh)
        {
            m = TreeMeshGenerator.GenerateTreeMesh(lines, 1.05f, radius, thinning);
            meshFilter.sharedMesh = m.CreateMesh();
        }
        else
            meshFilter.sharedMesh = new Mesh();
    }

    public void GenerateSentence()
    {
        string newSentence = "";
        for (int i = 0; i < sentence.Length; i++)
        {
            char current = sentence[i];
            string replacement = CheckRules(current);

            // Sentence building
            if (replacement != null)
                newSentence += replacement;
            else
                newSentence += current;
        }

        sentence = newSentence;
    }

    string CheckRules(char c)
    {
        foreach (Rule rule in rules)
        {
            if (c == rule.c)
                return rule.s;
        }
        return null;
    }

    public void GenerateTreeLSystem()
    {
        float tempBranchLen = branchLen;
        for (int i = 0; i < sentence.Length; i++)
        {
            char current = sentence[i];

            switch (current)
            {
                case 'F': // move forward at distance L(Step Length) and draw a line
                    Vector3 newPosition = position + (direction.normalized * tempBranchLen);
                    lines.Add(new Line(position, newPosition));
                    position = newPosition;
                    break;
                case 'f': // move forward at distance L(Step Length) without drawing a line
                    Vector3 newPosition_ = position + (direction.normalized * tempBranchLen);
                    position = newPosition_;
                    break;
                case '+': // turn left A(Default Angle) degrees (y-axis)
                    direction = Quaternion.Euler(0, -angle, 0) * direction;
                    break;
                case '-': // turn right A(Default Angle) degrees (y-axis)
                    direction = Quaternion.Euler(0, angle, 0) * direction;
                    break;
                case '\\': // turn left A(Default Angle) degrees (z-axis)
                    direction = Quaternion.Euler(0, 0, -angle) * direction;
                    break;
                case '/': // turn right A(Default Angle) degrees (z-axis)
                    direction = Quaternion.Euler(0, 0, angle) * direction;
                    break;
                case '^': // turn right A(Default Angle) degrees (x-axis)
                    direction = Quaternion.Euler(angle, 0, 0) * direction;
                    break;
                case '&': // turn left A(Default Angle) degrees (x-axis)
                    direction = Quaternion.Euler(-angle, 0, 0) * direction;
                    break;
                case '|':
                    direction = Quaternion.Euler(0, 180, 0) * direction;
                    break;
                case '[':
                    point.Push(new Point(position, direction));
                    break;
                case ']':
                    Point prevPoint = (Point)point.Pop();
                    position = prevPoint.position;
                    direction = prevPoint.direction;
                    break;
                default:
                    break;
            }

            tempBranchLen *= trimming;
        }
    }

    public void Reset()
    {
        sentence = "";
        lines.Clear();
        position = origin;
        m = null;
        branchLen = 5f;
        direction = Vector3.up;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (drawWireFrame)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Gizmos.DrawLine(lines[i].pos1, lines[i].pos2);
            }
            if (m != null)
            {
                for (int j = 0; j < m.vertices.Length; j++)
                {
                    Gizmos.DrawSphere(m.vertices[j], .1f);
                }
            }
        }
    }
}

public struct Point
{
    public Vector3 position;
    public Vector3 direction;

    public Point(Vector3 pos, Vector3 dir)
    {
        this.position = pos;
        this.direction = dir;
    }
}

public struct Line
{
    public Vector3 pos1;
    public Vector3 pos2;

    public Line(Vector3 p1, Vector3 p2)
    {
        this.pos1 = p1;
        this.pos2 = p2;
    }
}
