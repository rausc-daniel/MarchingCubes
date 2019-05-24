using System;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle
{
    public Vector3[] Vertices;

    public Triangle(Vector3[] vertices)
    {
        Vertices = vertices;
    }
}

public class Cube : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float isoValue;

    private int[] active;
    private Vector3[] corners;
    private MeshFilter _filter;

    private void Awake()
    {
        active = new int[8];
        corners = new Vector3[12];
        _filter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        AddCorner(new Vector3(-1, -1, -1), 0);
        AddCorner(new Vector3(-1, 1, -1), 1);
        AddCorner(new Vector3(1, 1, -1), 2);
        AddCorner(new Vector3(1, -1, -1), 3);
        AddCorner(new Vector3(-1, -1, 1), 4);
        AddCorner(new Vector3(-1, 1, 1), 5);
        AddCorner(new Vector3(1, 1, 1), 6);
        AddCorner(new Vector3(1, -1, 1), 7);
    }

    private void Update()
    {
        foreach (var corner in FindObjectsOfType<Corner>())
        {
            if (corner.Value > isoValue && corner.State == State.Inactive)
            {
                CornerToggle(corner.Id, corner.GetComponent<MeshRenderer>());
                corner.State = State.Active;
            }
            else if (corner.Value < isoValue && corner.State == State.Active)
            {
                CornerToggle(corner.Id, corner.GetComponent<MeshRenderer>());
                corner.State = State.Inactive;
            }
        }
    }

    private void AddCorner(Vector3 position, int id)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.SetParent(transform);
        go.transform.position = transform.position + position;
        go.transform.localScale = Vector3.one * 0.5f;
        var corner = go.AddComponent<Corner>();
        corner.Id = id;
        corner.OnClick = CornerToggle;
        corners[id] = position;
    }

    private List<Triangle> triangles;

    private void CornerToggle(int id, MeshRenderer renderer)
    {
        active[id] = (active[id] + 1) % 2;
        renderer.material.color = active[id] == 1 ? Color.red : Color.white;
        active.Log();
        int edgeIndex = Extensions.EdgeTable[active.Index()];
        var vertList = new Vector3[12];
        for (int i = 0; i < 12; i++)
        {
            if ((edgeIndex & Mathf.RoundToInt(Mathf.Pow(2, i))) != 0)
            {
                vertList[i] = Vector3.Lerp(corners[i % 8], corners[Extensions.ValueTable[i]], 0.5f);
                Debug.Log(corners[i % 8]);
                Debug.Log(corners[Extensions.ValueTable[i]]);
            }
        }

        int current = 0;
        triangles = new List<Triangle>();
        for (int i = 0; Extensions.TriTable[active.Index()][i] != -1; i += 3)
        {
            try
            {
                var p0 = vertList[Extensions.TriTable[active.Index()][i]];
                var p1 = vertList[Extensions.TriTable[active.Index()][i + 1]];
                var p2 = vertList[Extensions.TriTable[active.Index()][i + 2]];
                triangles.Add(new Triangle(new[] {p0, p1, p2}));
            }
            catch (IndexOutOfRangeException)
            {
            }
        }

        RenderMesh(triangles.GetVertices());
    }

    private void RenderMesh(Vector3[] vertices)
    {
        var mesh = new Mesh();
        mesh.vertices = vertices;
        var triangles = new int[vertices.Length - vertices.Length % 3];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            triangles[i] = i + 0;
            triangles[i + 1] = i + 2;
            triangles[i + 2] = i + 1;
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        _filter.mesh = mesh;
    }

//    private void OnDrawGizmos()
//    {
//        if (triangles == null) return;
//
//        foreach (var pos in triangles)
//        {
//            foreach (var vertex in pos.Vertices)
//            {
//                if (vertex == default) continue;
//
//                Gizmos.DrawWireSphere(transform.position + vertex, 0.1f);
//            }
//        }
//    }
}