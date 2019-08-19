using UnityEngine;

public class ChunkBehaviour : MonoBehaviour
{
    private Chunk chunk;
    private Vector3[] meshVertices;

    public MeshFilter Filter { get; private set; }
    public MeshRenderer Renderer { get; private set; }
    public BoxCollider Collider { get; private set; }

    private void Awake() {
        Filter = gameObject.AddComponent<MeshFilter>();
        Renderer = gameObject.AddComponent<MeshRenderer>();
        Collider = gameObject.AddComponent<BoxCollider>();
    }
    
    public void Init(Chunk chunk)
    {
        this.chunk = chunk;
        this.meshVertices = chunk.MeshVertices;
        
        Collider.isTrigger = true;
        Collider.center = chunk.Center;
        Collider.size = chunk.Size * Vector3.one;
    }
}
