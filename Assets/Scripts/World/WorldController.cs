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

    public void GenerateWorld()
    {
        if(anchor != null)
        {
            Destroy(anchor.gameObject);
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
                    CreateChunk(new Vector3(x, y, z), anchor);
                }
            }
        }
    }

    private void GenerateDynamicWorld()
    {
        foreach (var offset in Extensions.CreateChunks(spread.x))
        {
            CreateChunk(offset, anchor);
        }
    }

    private void CreateChunk(Vector3 offset, Transform parent)
    {
        var go = new GameObject($"Chunk [{offset.x}, {offset.y}, {offset.z}]");
        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();
        go.transform.SetParent(anchor);
        renderer.material = material;
        var chunk = new Chunk(ChunkSize, ChunkRes, offset, Extensions.Noise, 1f);
        chunk.CalculatePoints();
        RenderMesh(chunk.March(0.5f), filter);
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

    private Vector3 cubePosition;
}