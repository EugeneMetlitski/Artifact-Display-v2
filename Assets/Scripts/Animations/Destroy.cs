using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float destroyInSeconds = 3.0f;

    void Start()
    {
        Destroy(gameObject, destroyInSeconds);
    }
}
