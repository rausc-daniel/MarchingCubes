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
        for (int x = 0; x < spread.x; x++)
        {
            for (int y = 0; y < spread.y; y++)
            {
                for (int z = 0; z < spread.z; z++)
                {
                    var go = new GameObject($"Chunk [{x}, {y}, {z}]");
                    var filter = go.AddComponent<MeshFilter>();
                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.material = material;
                    var chunk = new Chunk(chunkSize, chunkRes, new Vector3(x, y, z), Extensions.Noise, 1);
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
}