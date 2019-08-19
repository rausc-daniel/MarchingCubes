using System.Collections.Generic;
using UnityEngine;

public enum WorldType
{
    Static,
    Dynamic
}

public class WorldController : MonoBehaviour
{
    public int ChunkSize;
    public int ChunkRes;
    public Vector3Int spread;
    public Material material;
    public WorldType Type;

    private Transform anchor;
    public Dictionary<Vector3, Vector3[]> chunkDict { get; } = new Dictionary<Vector3, Vector3[]>();

    private void Start()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        if (anchor != null)
        {
            DestroyImmediate(anchor.gameObject);
        }

        anchor = new GameObject("WorldAnchor").transform;

        switch (Type)
        {
            case WorldType.Static:
                GenerateStaticWorld();
                break;
            case WorldType.Dynamic:
                GenerateDynamicWorld();
                break;
        }
    }

    private void GenerateStaticWorld()
    {
        for (var x = 0; x < spread.x; x++)
        {
            for (var y = 0; y < spread.y; y++)
            {
                for (var z = 0; z < spread.z; z++)
                {
                    CreateChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    private void GenerateDynamicWorld()
    {
        foreach (var offset in Extensions.GetInitialChunkPositions(spread.x))
        {
            CreateChunk(offset);
        }
    }

    private void CreateChunk(Vector3 offset)
    {
        var chunkBehaviour = SetupChunkObject(offset);

        var center = ((Vector3) spread / 2f).Mul(offset * ChunkSize);
        var chunk = new Chunk(ChunkSize, ChunkRes, center, offset, Extensions.Noise, 1f);
        chunk.March(0.5f);

        chunkDict.Add(offset, chunk.MeshVertices);

        chunkBehaviour.Init(chunk);
        RenderMesh(chunk.MeshVertices, chunkBehaviour.GetComponent<MeshFilter>());
    }

    private ChunkBehaviour SetupChunkObject(Vector3 offset){
        var chunkObject = new GameObject($"Chunk [{offset.x}, {offset.y}, {offset.z}]");
        chunkObject.transform.SetParent(anchor);

        var chunkBehaviour = chunkObject.AddComponent<ChunkBehaviour>();

        chunkBehaviour.Renderer.material = material;

        return chunkBehaviour;
    }

    private void RenderMesh(Vector3[] vertices, MeshFilter filter)
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

        filter.mesh = mesh;

        var collider = filter.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }
}
