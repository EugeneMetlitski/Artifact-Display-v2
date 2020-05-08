using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class auto : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        RestartGameInvoke();
    }

    void RestartGameInvoke()
    {
        if(Input.anyKeyDown)
        {
            CancelInvoke();
        }
        else
        {
            Invoke ("ResetScene", 10);
        }
    }

    void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
