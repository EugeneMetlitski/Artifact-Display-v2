using UnityEngine;

public class Distructible : MonoBehaviour
{
    public GameObject destroyed;

    void OnMouseDown()
    {
        Instantiate(destroyed, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
