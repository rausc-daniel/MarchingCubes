using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldController : MonoBehaviour
{
    [SerializeField] private int size;
    [SerializeField] private float scale;
    [SerializeField, Range(0, 1)] private float isoValue;

    private float[] values;
    private GameObject[] nodes;


    private void Awake()
    {
        values = new float[Mathf.RoundToInt(Mathf.Pow(size, 3))];
        nodes = new GameObject[Mathf.RoundToInt(Mathf.Pow(size, 3))];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    int index = GetIndex(x, y, z);
                    var pos = new Vector3(x, y, z) * scale;
                    values[index] = Random.Range(0f, 1f);
                    var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    obj.transform.position = pos;
                    nodes[x + size * y + size * size * z] = obj;
                }
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    nodes[GetIndex(x * 0, y * 0, z * 0)].GetComponent<MeshRenderer>().material.color = Color.red;
                    nodes[GetIndex(x * 1, y * 0, z * 0)].GetComponent<MeshRenderer>().material.color = Color.red;
                    nodes[GetIndex(x * 1, y * 1, z * 0)].GetComponent<MeshRenderer>().material.color = Color.red;
                    nodes[GetIndex(x * 1, y * 1, z * 1)].GetComponent<MeshRenderer>().material.color = Color.red;
                    nodes[GetIndex(x * 0, y * 1, z * 1)].GetComponent<MeshRenderer>().material.color = Color.red;
                    nodes[GetIndex(x * 0, y * 0, z * 1)].GetComponent<MeshRenderer>().material.color = Color.red;
                    nodes[GetIndex(x * 1, y * 0, z * 1)].GetComponent<MeshRenderer>().material.color = Color.red;
                    nodes[GetIndex(x * 0, y * 1, z * 0)].GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] < isoValue)
            {
                nodes[i].SetActive(false);
            }
            else
            {
                nodes[i].SetActive(true);
            }
        }
    }

    private int GetIndex(int x, int y, int z) => size * size * z + size * y + x;
}