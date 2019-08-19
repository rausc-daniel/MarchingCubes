using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        GetComponent<WorldController>().GenerateWorld();
    }
}
