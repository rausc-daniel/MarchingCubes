using System;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle
{
    public readonly Vector3[] Vertices;

    public Triangle(Vector3[] vertices)
    {
        Vertices = vertices;
    }
}

public class Cube : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float isoValue = default;

    private int[] _active;
    private Corner[] _corners;
    private MeshFilter _filter;

    private void Awake()
    {
        _active = new int[8];
        _corners = new Corner[8];
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
        if (_corners is null)
            return;

        foreach (var corner in _corners)
        {
            if (corner is null)
                continue;

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
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.5f;
        var corner = go.AddComponent<Corner>();
        corner.Id = id;
        corner.OnClick = CornerToggle;
        _corners[id] = corner;
    }

    private List<Triangle> triangles;

    private void CornerToggle(int id, MeshRenderer renderer)
    {
        _active[id] = (_active[id] + 1) % 2;
        renderer.material.color = _active[id] == 1 ? Color.red : Color.white;
        var edgeIndex = Extensions.EdgeTable[_active.Index()];
        var vertList = new Vector3[12];
        for (var i = 0; i < 12; i++)
        {
            if ((edgeIndex & Mathf.RoundToInt(Mathf.Pow(2, i))) != 0)
            {
                vertList[i] = Vector3.Lerp(_corners[i % 8].transform.position,
                    _corners[Extensions.ValueTable[i]].transform.position, 0.5f);
            }
        }

        triangles = new List<Triangle>();
        for (var i = 0; Extensions.TriTable[_active.Index()][i] != -1; i += 3)
        {
            var p0 = vertList[Extensions.TriTable[_active.Index()][i]];
            var p1 = vertList[Extensions.TriTable[_active.Index()][i + 1]];
            var p2 = vertList[Extensions.TriTable[_active.Index()][i + 2]];
            triangles.Add(new Triangle(new[] {p0, p1, p2}));
        }

        RenderMesh(triangles.GetVertices());
    }

    private void RenderMesh(Vector3[] vertices)
    {
        var mesh = new Mesh();
        mesh.vertices = vertices;
        var triangles = new int[vertices.Length - vertices.Length % 3];
        for (var i = 0; i < triangles.Length; i += 3)
        {
            triangles[i] = i + 0;
            triangles[i + 1] = i + 2;
            triangles[i + 2] = i + 1;
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        _filter.mesh = mesh;
    }
}