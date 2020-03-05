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

    /// <summary>
    /// Initialize a new Chunk
    /// </summary>
    /// <param name="size">the width, height and depth of a chunk in world unity</param>
    /// <param name="resolution">how many nodes each chunk should have</param>
    /// <param name="offset">where the chunk should be positioned relative to the origin</param>
    /// <param name="noiseFunc">the function that generates the noise</param>
    /// <param name="noiseSize">the resolution of the noise</param>
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

    /// <summary>
    /// Calculate where each node should be both in the world and in the noise
    /// </summary>
    public void CalculatePoints()
    {
        for (var x = 0; x < resolution; x++)
        {
            for (var y = 0; y < resolution; y++)
            {
                for (var z = 0; z < resolution; z++)
                {
                    var index = GetIndex(x, y, z);
                    var pos = offset * (size - (float) size / resolution) + new Vector3(x, y, z) * size / resolution;
                    nodes[index] = pos;
                    var noiseOffset = offset * (noiseSize - noiseSize / resolution) +
                                      new Vector3(x, y, z) * noiseSize / resolution;
                    values[index] = noiseFunc(noiseOffset.x, noiseOffset.y, noiseOffset.z);
                }
            }
        }
    }

    /// <summary>
    ///  Applies the Marching Cubes Algorithm for the calculated points
    /// </summary>
    /// <param name="isoLevel">the threshold for the marching cubes algorithm</param>
    /// <returns>an array of vertices the mesh is comprised of</returns>
    public Vector3[] March(float isoLevel)
    {
        var vertices = new List<Vector3>();

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

                    var corners = new Vector3[8];
                    var cubeIndex = 0;
                    for (var i = 0; i < 8; i++)
                    {
                        corners[i] = nodes[indices[i]];
                        if (values[indices[i]] > isoLevel)
                        {
                            cubeIndex |= Mathf.RoundToInt(Mathf.Pow(2, i));
                        }
                    }

                    var edgeIndex = Extensions.EdgeTable[cubeIndex];

                    var vertList = new Vector3[12];
                    for (var i = 0; i < 12; i++)
                    {
                        if ((edgeIndex & Mathf.RoundToInt(Mathf.Pow(2, i))) != 0)
                        {
                            vertList[i] = InterpolateVerts(isoLevel, corners[i % 8], corners[Extensions.ValueTable[i]],
                                values[indices[i % 8]], values[indices[Extensions.ValueTable[i]]]);
                        }
                    }

                    for (var i = 0; Extensions.TriTable[cubeIndex][i] != -1; i += 3)
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

    /// <summary>
    /// Considers the values of the nodes for placement of vertices
    /// </summary>
    /// <param name="isoLevel">the threshold for the marching cubes algorithm</param>
    /// <param name="p1">the lower bound of the interpolation</param>
    /// <param name="p2">the upper bound of the interpolation</param>
    /// <param name="v1">the value of the lower bound</param>
    /// <param name="v2">the value of the upper bound</param>
    /// <returns></returns>
    private Vector3 InterpolateVerts(float isoLevel, Vector3 p1, Vector3 p2, float v1, float v2)
    {
        var t = (isoLevel - v1) / (v2 - v1);
        return p1 + t * (p2 - p1);
    }

    /// <summary>
    /// Transform a 3-dimensional point into a 1-dimensional array index
    /// </summary>
    /// <param name="x">x-coordinate</param>
    /// <param name="y">y-coordinate</param>
    /// <param name="z">z-coordinate</param>
    /// <returns>the index in a 1-dimensional array</returns>
    private int GetIndex(int x, int y, int z) => resolution * resolution * z + resolution * y + x;
}