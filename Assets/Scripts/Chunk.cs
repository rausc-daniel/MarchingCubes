using System;
using System.Collections.Generic;
using UnityEngine;

public struct Chunk
{
    private readonly int size;
    private readonly int resolution;
    private readonly Vector3 offset;
    private readonly Func<float, float, float, float> noiseFunc;
    private readonly float noiseSize;

    private readonly float[] values;
    private readonly Vector3[] nodes;

    public Chunk(int size, int resolution, Vector3 offset, Func<float, float, float, float> noiseFunc, float noiseSize)
    {
        this.size = size + 1;
        this.resolution = resolution + 1;
        this.offset = offset;
        this.noiseFunc = noiseFunc;
        this.noiseSize = noiseSize;

        values = new float[Mathf.RoundToInt(Mathf.Pow(this.resolution, 3))];
        nodes = new Vector3[Mathf.RoundToInt(Mathf.Pow(this.resolution, 3))];
    }

    public void CalculatePoints()
    {
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    int index = GetIndex(x, y, z);
                    var pos = offset * (size - (float) size / resolution) + new Vector3(x, y, z) * size / resolution;
                    nodes[index] = pos;
                    var noiseOffset = offset * (noiseSize - noiseSize / resolution) +
                                      new Vector3(x, y, z) * noiseSize / resolution;
                    values[index] = noiseFunc(noiseOffset.x, noiseOffset.y, noiseOffset.z);
                }
            }
        }
    }

    public Vector3[] March(float isoLevel)
    {
        var vertices = new List<Vector3>();

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
                }
            }
        }

        return vertices.ToArray();
    }

    private Vector3 InterpolateVerts(float isoLevel, Vector3 p1, Vector3 p2, float v1, float v2)
    {
        float t = (isoLevel - v1) / (v2 - v1);
        return p1 + t * (p2 - p1);
    }

    private int GetIndex(int x, int y, int z) => resolution * resolution * z + resolution * y + x;
}