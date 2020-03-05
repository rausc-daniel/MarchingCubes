using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization : MonoBehaviour
{
    [SerializeField] private int size = default;
    [SerializeField] private int resolution = default;
    [SerializeField] private Material material = default;
    [SerializeField] private float isoLevel = default;
    [SerializeField] private float delay = default;
    [SerializeField] private float noiseSize = default;

    private Vector3 _offset = Vector3.zero;
    private Func<float, float, float, float> _noiseFunc = Extensions.Noise;

    private MeshFilter _filter;
    private MeshRenderer _renderer;

    private float[] _values;
    private Vector3[] _nodes;

    private Vector3 _cubePosition;
    private List<Vector3> _vertices;
    private IEnumerator _marchEnumerator;

    private void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();

        _renderer.material = material;
    }

    public void CalculatePoints()
    {
        _values = new float[Mathf.RoundToInt(Mathf.Pow(resolution, 3))];
        _nodes = new Vector3[Mathf.RoundToInt(Mathf.Pow(resolution, 3))];

        for (var x = 0; x < resolution; x++)
        {
            for (var y = 0; y < resolution; y++)
            {
                for (var z = 0; z < resolution; z++)
                {
                    var index = GetIndex(x, y, z);
                    var pos = _offset * (size - (float) size / resolution) + new Vector3(x, y, z) * size / resolution;
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = pos;
                    go.transform.localScale = Vector3.one * 0.1f;
                    go.transform.SetParent(transform);
                    _nodes[index] = pos;
                    var noiseOffset = _offset * (noiseSize - noiseSize / resolution) +
                                      new Vector3(x, y, z) * noiseSize / resolution;
                    _values[index] = _noiseFunc(noiseOffset.x, noiseOffset.y, noiseOffset.z);
                }
            }
        }
        
        _marchEnumerator = March();
    }

    public void Step()
    {
        _marchEnumerator.MoveNext();
    }

    public void Visualize()
    {
        IEnumerator March()
        {
            while (true)
            {
                Step();
                yield return new WaitForSeconds(delay);
            }
        }

        StartCoroutine(March());
    }

    private IEnumerator March()
    {
        _vertices = new List<Vector3>();

        for (var x = 0; x < resolution - 1; x++)
        {
            for (var y = 0; y < resolution - 1; y++)
            {
                for (var z = 0; z < resolution - 1; z++)
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

                    _cubePosition = Vector3.Lerp(_nodes[indices[0]], _nodes[indices[6]], 0.5f);

                    var corners = new Vector3[8];
                    var cubeIndex = 0;
                    for (var i = 0; i < 8; i++)
                    {
                        corners[i] = _nodes[indices[i]];
                        if (_values[indices[i]] > isoLevel)
                            cubeIndex |= Mathf.RoundToInt(Mathf.Pow(2, i));
                    }

                    var edgeIndex = Extensions.EdgeTable[cubeIndex];

                    var vertList = new Vector3[12];
                    for (var i = 0; i < 12; i++)
                    {
                        if ((edgeIndex & Mathf.RoundToInt(Mathf.Pow(2, i))) != 0)
                        {
                            vertList[i] = InterpolateVerts(isoLevel, corners[i % 8], corners[Extensions.ValueTable[i]],
                                _values[indices[i % 8]], _values[indices[Extensions.ValueTable[i]]]);
                        }
                    }

                    for (var i = 0; Extensions.TriTable[cubeIndex][i] != -1; i += 3)
                    {
                        _vertices.Add(vertList[Extensions.TriTable[cubeIndex][i]]);
                        _vertices.Add(vertList[Extensions.TriTable[cubeIndex][i + 1]]);
                        _vertices.Add(vertList[Extensions.TriTable[cubeIndex][i + 2]]);
                    }

                    RenderMesh(_vertices.ToArray(), _filter);
                    yield return null;
                }
            }
        }
    }

    private void RenderMesh(Vector3[] vertices, MeshFilter filter)
    {
        var mesh = new Mesh {vertices = vertices};
        var triangles = new int[vertices.Length - vertices.Length % 3];
        for (var i = 0; i < triangles.Length; i += 3)
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
        var t = (isoLevel - v1) / (v2 - v1);
        return p1 + t * (p2 - p1);
    }

    private int GetIndex(int x, int y, int z) => resolution * resolution * z + resolution * y + x;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(_cubePosition, Vector3.one * size / resolution);
        
        if(_vertices is null)
            return;
        
        Gizmos.color = Color.red;
        foreach (var vertex in _vertices)
        {
            Gizmos.DrawWireSphere(vertex, 0.1f);
        }
    }
}