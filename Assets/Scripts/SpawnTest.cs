using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private int radius;

    private void Start()
    {
        var i = 0;
        
        
        
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = anchor.transform.position + new Vector3(0, -radius, 0);

        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = anchor.transform.position + new Vector3(0, -radius + 1, -1);
        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = anchor.transform.position + new Vector3(-1, -radius + 1, 0);
        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = anchor.transform.position + new Vector3(0, -radius + 1, 0);
        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = anchor.transform.position + new Vector3(1, -radius + 1, 0);
        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = anchor.transform.position + new Vector3(0, -radius + 1, 1);
    }
}