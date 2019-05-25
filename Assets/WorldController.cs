using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldController : MonoBehaviour
{
    [SerializeField] private int size;
    [SerializeField] private float scale;
    [SerializeField, Range(0, 1)] private float isoValue;

    private float[] values;
    private Vector3[] nodes;


    private void Awake()
    {
        size++;
        
        values = new float[Mathf.RoundToInt(Mathf.Pow(size, 3))];
        nodes = new Vector3[Mathf.RoundToInt(Mathf.Pow(size, 3))];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    int index = GetIndex(x, y, z);
                    var pos = new Vector3(x, y, z) * scale;
                    values[index] = Random.Range(0f, 1f);
//                    var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//                    obj.transform.position = pos;
                    nodes[x + size * y + size * size * z] = pos;
                }
            }
        }
    }

    private void Update()
    {
        var triangles = new List<Triangle>();

        for (int x = 0; x < size - 1; x++)
        {
            for (int y = 0; y < size - 1; y++)
            {
                for (int z = 0; z < size - 1; z++)
                {
                    var indices = new int[]
                    {
                        GetIndex(x + 0, y + 0, z + 0),
                        GetIndex(x + 0, y + 1, z + 0),
                        GetIndex(x + 1, y + 1, z + 0),
                        GetIndex(x + 1, y + 0, z + 0),
                        GetIndex(x + 0, y + 0, z + 1),
                        GetIndex(x + 0, y + 1, z + 1),
                        GetIndex(x + 1, y + 1, z + 1),
                        GetIndex(x + 1, y + 0, z + 1)
                    };

                    var corners = new Vector3[8];
                    int cubeIndex = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        corners[i] = nodes[indices[i]];
                        if (values[indices[i]] > isoValue)
                            cubeIndex |= Mathf.RoundToInt(Mathf.Pow(2, i));
                    }

                    int edgeIndex = Extensions.EdgeTable[cubeIndex];

                    var vertList = new Vector3[12];
                    for (int i = 0; i < 12; i++)
                    {
                        if ((edgeIndex & Mathf.RoundToInt(Mathf.Pow(2, i))) != 0)
                        {
                            vertList[i] = Vector3.Lerp(corners[i % 8],
                                corners[Extensions.ValueTable[i]], 0.5f);
                        }
                    }

                    int current = 0;

                    for (int i = 0; Extensions.TriTable[cubeIndex][i] != -1; i += 3)
                    {
                        var p0 = vertList[Extensions.TriTable[cubeIndex][i]];
                        var p1 = vertList[Extensions.TriTable[cubeIndex][i + 1]];
                        var p2 = vertList[Extensions.TriTable[cubeIndex][i + 2]];
                        triangles.Add(new Triangle(new[] {p0, p1, p2}));
                    }
                }
            }
        }

        RenderMesh(triangles.GetVertices());
    }

    private MeshFilter _filter;

    private void RenderMesh(Vector3[] vertices)
    {
        _filter = GetComponent<MeshFilter>();

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

//    private void Update()
//    {
//        for (int i = 0; i < values.Length; i++)
//        {
//            if (values[i] < isoValue)
//            {
//                nodes[i].SetActive(false);
//            }
//            else
//            {
//                nodes[i].SetActive(true);
//            }
//        }
//    }

    private int GetIndex(int x, int y, int z) => size * size * z + size * y + x;
}