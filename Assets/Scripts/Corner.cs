using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum State
{
    Inactive,
    Active
}

public class Corner : MonoBehaviour
{
    public int Id;
    public Action<int, MeshRenderer> OnClick;
    public float Value;
    public State State;

    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        Value = Random.Range(0f, 1f);
    }

    private void OnMouseDown()
    {
        OnClick(Id, _renderer);
    }
}