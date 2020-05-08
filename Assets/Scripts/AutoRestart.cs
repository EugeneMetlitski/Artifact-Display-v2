using UnityEngine.SceneManagement;
using UnityEngine;

public class AutoRestart : MonoBehaviour
{
    public int numSecondsBeforeRestart = 10;
    public string mainSceneName = "Main";

    void Update()
    {
        RestartGameInvoke();
    }

    private void RestartGameInvoke()
    {
        if(Input.anyKeyDown)
        {
            CancelInvoke();
        }
        else
        {
            Invoke ("ResetScene", numSecondsBeforeRestart);
        }
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}
