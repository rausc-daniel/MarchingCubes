using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Visualization
{
    public enum State
    {
        Inactive,
        Active
    }

    public class Corner : MonoBehaviour
    {
        public int Id { get; set; }
        public Action<int, MeshRenderer> OnClick { get; set; }
        public float Value { get; private set; }
        public State State { get; set; }

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
}