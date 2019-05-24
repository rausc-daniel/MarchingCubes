using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private int[] active;
    private Vector3[] corners;

    private void Awake()
    {
        active = new int[8];
        corners = new Vector3[12];
    }

    private void Start()
    {
        AddCorner(new Vector3(-1, -1, -1), 0);
        AddCorner(new Vector3(-1, 1, -1), 1);
        AddCorner(new Vector3(1, 1, -1), 2);
        AddCorner(new Vector3(1, -1, -1), 3);
        AddCorner(new Vector3(-1, -1, 1), 4);
        AddCorner(new Vector3(-1, 1, 1), 5);
        AddCorner(new Vector3(1, 1, 1), 6);
        AddCorner(new Vector3(1, -1, 1), 7);
    }

    private void AddCorner(Vector3 position, int id)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.SetParent(transform);
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.5f;
        var corner = go.AddComponent<Corner>();
        corner.Id = id;
        corner.OnClick = CornerToggle;
        corners[id] = position;
    }

    private Vector3[] vertices;

    private void CornerToggle(int id, MeshRenderer renderer)
    {
        active[id] = (active[id] + 1) % 2;
        renderer.material.color = active[id] == 1 ? Color.red : Color.white;
        active.Log();
        int edgeIndex = Extensions.EdgeTable[active.Index()];
        var vertList = new Vector3[12];
        for (int i = 0; i < 12; i++)
        {
            if ((edgeIndex & Mathf.RoundToInt(Mathf.Pow(2, i))) != 0)
            {
                vertList[i] = Vector3.Lerp(corners[i % 8], corners[Extensions.ValueTable[i]], 0.5f);
                Debug.Log(corners[i % 8]);
                Debug.Log(corners[Extensions.ValueTable[i]]);
            }
        }

        int current = 0;
        vertices = new Vector3[12];
        for (int i = 0; Extensions.TriTable[active.Index()][i] != -1; i++)
        {
            vertices[i] = vertList[Extensions.TriTable[active.Index()][i]];
        }
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        foreach (var pos in vertices)
        {
            if(pos == default) continue;
            
            Gizmos.DrawWireSphere(pos, 0.1f);
        }
    }
}