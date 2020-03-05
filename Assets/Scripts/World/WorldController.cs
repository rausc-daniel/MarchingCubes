using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField] private int chunkSize = default;
    [SerializeField] private int chunkRes = default;

    [SerializeField, Tooltip("How many Chunks should be generated in each direction")]
    private Vector3Int spread = default;

    [SerializeField] private Material material = default;

    [SerializeField, Range(0, 1)] private float isoLevel = default;

    private void Awake()
    {
        for (var x = 0; x < spread.x; x++)
        {
            for (var y = 0; y < spread.y; y++)
            {
                for (var z = 0; z < spread.z; z++)
                {
                    var go = new GameObject($"Chunk [{x}, {y}, {z}]");
                    var filter = go.AddComponent<MeshFilter>();
                    var meshRenderer = go.AddComponent<MeshRenderer>();
                    meshRenderer.material = material;
                    var chunk = new Chunk(chunkSize, chunkRes, new Vector3(x, y, z), Extensions.Noise, 2);
                    chunk.CalculatePoints();
                    RenderMesh(chunk.March(isoLevel), filter);
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
        var meshCollider = filter.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}