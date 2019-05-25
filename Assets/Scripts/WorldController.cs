using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class WorldController : MonoBehaviour
{
//    [SerializeField] private int size;
//    [SerializeField] private int resolution;
//    [SerializeField, Range(0, 5)] private int offset;
//    [SerializeField, Range(0, 1)] private float noiseSize;
//    [SerializeField, Range(0, 1)] private float isoValue;
//
//    private float[] values;
//    private Vector3[] nodes;
    private MeshFilter _filter;

//
//    private void Awake()
//    {
//        size++;
//        resolution++;
//
//        var noise = new FastNoise();
//
//        values = new float[Mathf.RoundToInt(Mathf.Pow(resolution, 3))];
//        nodes = new Vector3[Mathf.RoundToInt(Mathf.Pow(resolution, 3))];
//
//        for (int x = 0; x < resolution; x++)
//        {
//            for (int y = 0; y < resolution; y++)
//            {
//                for (int z = 0; z < resolution; z++)
//                {
//                    var pos = new Vector3(x, y, z) * size / resolution;
////                    var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
////                    obj.transform.position = pos;
//                    nodes[x + resolution * y + resolution * resolution * z] = pos;
//                }
//            }
//        }
//
//        Generate();
//    }
//
    private float Noise(float x, float y, float z)
    {
        return (Mathf.PerlinNoise(x, y) + Mathf.PerlinNoise(y, x) +
                Mathf.PerlinNoise(x, z) + Mathf.PerlinNoise(z, x) +
                Mathf.PerlinNoise(y, z) + Mathf.PerlinNoise(z, y)) / 6;
    }
//
//    private void Update()
//    {
//        Generate();
//    }
//
//    private void Generate()
//    {
//        var triangles = new List<Triangle>();
//
//        for (int x = 0; x < resolution - 1; x++)
//        {
//            for (int y = 0; y < resolution - 1; y++)
//            {
//                for (int z = 0; z < resolution - 1; z++)
//                {
//                    var indices = new int[]
//                    {
//                        GetIndex(x + 0, y + 0, z + 0),
//                        GetIndex(x + 0, y + 1, z + 0),
//                        GetIndex(x + 1, y + 1, z + 0),
//                        GetIndex(x + 1, y + 0, z + 0),
//                        GetIndex(x + 0, y + 0, z + 1),
//                        GetIndex(x + 0, y + 1, z + 1),
//                        GetIndex(x + 1, y + 1, z + 1),
//                        GetIndex(x + 1, y + 0, z + 1)
//                    };
//
//                    int index = GetIndex(x, y, z);
//                    values[index] = Noise(offset + (float) x / resolution / noiseSize,
//                        offset + (float) y / resolution / noiseSize,
//                        offset + (float) z / resolution / noiseSize);
//
//                    cubePosition = Vector3.Lerp(nodes[indices[0]], nodes[indices[6]], 0.5f);
//
//                    var corners = new Vector3[8];
//                    int cubeIndex = 0;
//                    for (int i = 0; i < 8; i++)
//                    {
//                        corners[i] = nodes[indices[i]];
//                        if (values[indices[i]] > isoValue)
//                            cubeIndex |= Mathf.RoundToInt(Mathf.Pow(2, i));
//                    }
//
//                    int edgeIndex = Extensions.EdgeTable[cubeIndex];
//
//                    var vertList = new Vector3[12];
//                    for (int i = 0; i < 12; i++)
//                    {
//                        if ((edgeIndex & Mathf.RoundToInt(Mathf.Pow(2, i))) != 0)
//                        {
//                            vertList[i] = InterpolateVerts(corners[i % 8], corners[Extensions.ValueTable[i]],
//                                values[indices[i % 8]], values[indices[Extensions.ValueTable[i]]]);
//                        }
//                    }
//
//                    int current = 0;
//
//                    for (int i = 0; Extensions.TriTable[cubeIndex][i] != -1; i += 3)
//                    {
//                        var p0 = vertList[Extensions.TriTable[cubeIndex][i]];
//                        var p1 = vertList[Extensions.TriTable[cubeIndex][i + 1]];
//                        var p2 = vertList[Extensions.TriTable[cubeIndex][i + 2]];
//                        triangles.Add(new Triangle(new[] {p0, p1, p2}));
//                    }
//                }
//            }
//        }
//
//        RenderMesh(triangles.GetVertices());
//    }
//
//    private Vector3 InterpolateVerts(Vector3 p1, Vector3 p2, float v1, float v2)
//    {
//        float t = (isoValue - v1) / (v2 - v1);
//        return p1 + t * (p2 - p1);
//    }
//
//    private Vector3[] verts;

    [SerializeField] private Material material;

    private void Awake()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    var go = new GameObject($"Chunk [{x}, {y}, {z}]");
                    var filter = go.AddComponent<MeshFilter>();
                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.material = material;
                    var chunk = new Chunk(10, 15, new Vector3(x, y, z), Noise, 1);
                    chunk.CalculatePoints();
                    RenderMesh(chunk.March(0.5f), filter);
                }
            }
        }
    }

    private void RenderMesh(Vector3[] vertices, MeshFilter filter)
    {
        Debug.Log(vertices.Length);

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

        filter.mesh = mesh;
    }

    private Vector3 cubePosition;

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

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//
//        foreach (var vert in verts)
//        {
//            Gizmos.DrawWireSphere(vert, 1f / resolution);
//        }
//    }

    //private int GetIndex(int x, int y, int z) => size * size * z + size * y + x;
}