using System;
using UnityEngine;
using UnityEngine.UI;

public class Corner : MonoBehaviour
{
    public int Id;
    public Action<int, MeshRenderer> OnClick;

    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    private void OnMouseDown()
    {
        OnClick(Id, _renderer);
    }
}