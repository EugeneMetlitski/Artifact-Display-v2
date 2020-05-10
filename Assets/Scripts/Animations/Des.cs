using UnityEngine;

public class Des : MonoBehaviour
{
    public float destroyInSeconds = 3.0f;

    void Start()
    {
        Destroy(gameObject, destroyInSeconds);
    }
}
