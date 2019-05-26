using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization : MonoBehaviour
{
    [SerializeField] private int size;
    [SerializeField] private int resolution;
    [SerializeField] private Material material;
    [SerializeField] private float isoLevel;
    [SerializeField] private float delay;

    private const float NoiseSize = 1f;

    private Vector3 offset = Vector3.zero;
    private Func<float, float, float, float> noiseFunc = Extensions.Noise;

    private MeshFilter _filter;
    private MeshRenderer _renderer;

    private float[] values;
    private Vector3[] nodes;

    private Vector3 cubePosition;
    private List<Vector3> vertices;
    private IEnumerator marchEnumerator;

    private void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();

        _renderer.material = material;
    }

    public void CalculatePoints()
    {
        values = new float[Mathf.RoundToInt(Mathf.Pow(resolution, 3))];
        nodes = new Vector3[Mathf.RoundToInt(Mathf.Pow(resolution, 3))];

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    int index = GetIndex(x, y, z);
                    var pos = offset * (size - (float) size / resolution) + new Vector3(x, y, z) * size / resolution;
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = pos;
                    go.transform.localScale = Vector3.one * 0.1f;
                    nodes[index] = pos;
                    var noiseOffset = offset * (NoiseSize - NoiseSize / resolution) +
                                      new Vector3(x, y, z) * NoiseSize / resolution;
                    values[index] = noiseFunc(noiseOffset.x, noiseOffset.y, noiseOffset.z);
                }
            }
        }
        
        marchEnumerator = March();
    }

    public void Step()
    {
        marchEnumerator.MoveNext();
    }

    public void Visualize()
    {
        IEnumerator march()
        {
            while (true)
            {
                Step();
                yield return new WaitForSeconds(delay);
            }
        }

        StartCoroutine(march());
    }

    public IEnumerator March()
    {
        vertices = new List<Vector3>();

        for (int x = 0; x < resolution - 1; x++)
        {
            for (int y = 0; y < resolution - 1; y++)
            {
                for (int z = 0; z < resolution - 1; z++)
                {
                    var indices = new[]
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

                    cubePosition = Vector3.Lerp(nodes[indices[0]], nodes[indices[6]], 0.5f);

                    var corners = new Vector3[8];
                    int cubeIndex = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        corners[i] = nodes[indices[i]];
                        if (values[indices[i]] > isoLevel)
                            cubeIndex |= Mathf.RoundToInt(Mathf.Pow(2, i));
                    }

                    int edgeIndex = Extensions.EdgeTable[cubeIndex];

                    var vertList = new Vector3[12];
                    for (int i = 0; i < 12; i++)
                    {
                        if ((edgeIndex & Mathf.RoundToInt(Mathf.Pow(2, i))) != 0)
                        {
                            vertList[i] = InterpolateVerts(isoLevel, corners[i % 8], corners[Extensions.ValueTable[i]],
                                values[indices[i % 8]], values[indices[Extensions.ValueTable[i]]]);
                        }
                    }

                    for (int i = 0; Extensions.TriTable[cubeIndex][i] != -1; i += 3)
                    {
                        vertices.Add(vertList[Extensions.TriTable[cubeIndex][i]]);
                        vertices.Add(vertList[Extensions.TriTable[cubeIndex][i + 1]]);
                        vertices.Add(vertList[Extensions.TriTable[cubeIndex][i + 2]]);
                    }

                    RenderMesh(vertices.ToArray(), _filter);
                    yield return null;
                }
            }
        }
    }

    private void RenderMesh(Vector3[] vertices, MeshFilter filter)
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

        filter.mesh = mesh;
    }

    private Vector3 InterpolateVerts(float isoLevel, Vector3 p1, Vector3 p2, float v1, float v2)
    {
        float t = (isoLevel - v1) / (v2 - v1);
        return p1 + t * (p2 - p1);
    }

    private int GetIndex(int x, int y, int z) => resolution * resolution * z + resolution * y + x;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var vertex in vertices)
        {
            Gizmos.DrawWireSphere(vertex, 0.1f);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(cubePosition, Vector3.one * size / resolution);
    }
}