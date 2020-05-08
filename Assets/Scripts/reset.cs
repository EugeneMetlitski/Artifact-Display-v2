using UnityEngine.SceneManagement;
using UnityEngine;

public class reset : MonoBehaviour
{
    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
