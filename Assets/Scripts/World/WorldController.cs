using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField] private int chunkSize;
    [SerializeField] private int chunkRes;

    [SerializeField, Tooltip("How many Chunks should be generated in each direction")]
    private Vector3Int spread;

    [SerializeField] private Material material;

    private void Awake()
    {
        GenerateDynamicWorld();
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
        foreach (var offset in Extensions.CreateChunks(spread.x))
        {
            CreateChunk(offset);
        }
    }

    private void CreateChunk(Vector3 offset)
    {
        var go = new GameObject($"Chunk [{offset.x}, {offset.y}, {offset.z}]");
        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.material = material;
        var chunk = new Chunk(chunkSize, chunkRes, offset, Extensions.Noise, 1f);
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