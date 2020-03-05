using System.Security.Cryptography;
using NaughtyAttributes;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField] private int chunkSize = default;
    [SerializeField] private int chunkRes = default;

    [SerializeField, Tooltip("How many Chunks should be generated in each direction")]
    private Vector3Int spread = default;

    [SerializeField] private Material material = default;

    [OnValueChanged("RegenerateMeshes")]
    [SerializeField, Range(0, 1)] private float isoLevel = default;
    
    private void Awake()
    {
        GenerateMeshes();
    }
    
    /// <summary>
    /// Callback for a NaughtyAttributes <see cref="OnValueChangedAttribute"/>. Sets the isoValue to
    /// <paramref name="newValue"/> and starts the mesh generation.
    /// <param name="oldValue">The value before the field was changed.</param>
    /// <param name="newValue">The new value after the change.</param>
    /// </summary>
    private void RegenerateMeshes(float oldValue, float newValue)
    {
        DestroyOldChunks();
        isoLevel = newValue;
        GenerateMeshes();
    }

    private void DestroyOldChunks()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void GenerateMeshes()
    {
        for (var x = 0; x < spread.x; x++)
        {
            for (var y = 0; y < spread.y; y++)
            {
                for (var z = 0; z < spread.z; z++)
                {
                    var go = new GameObject($"Chunk [{x}, {y}, {z}]");
                    go.transform.SetParent(transform);
                    var filter = go.AddComponent<MeshFilter>();
                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.material = material;
                    var chunk = new Chunk(chunkSize, chunkRes, new Vector3(x, y, z), Extensions.Noise, 2);
                    chunk.CalculatePoints();
                    RenderMesh(chunk.March(isoLevel), filter);
                }
            }
        }
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
        collider.sharedMesh = null;
        collider.sharedMesh = mesh;
    }

    private Vector3 cubePosition;
}