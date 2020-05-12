using UnityEngine;

public class Distructible : MonoBehaviour
{
    public GameObject destroyed;

    public void Destroy()
    {
        Instantiate(destroyed, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
