using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    public string mainSceneName = "Main";

    public void OnClick()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}
