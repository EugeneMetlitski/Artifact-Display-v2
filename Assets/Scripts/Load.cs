using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Vuforia;

public class Load : MonoBehaviour
{
    public void Restart(){
        SceneManager.LoadScene("Assets/Scenes/Main.unity");
        TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
    }
}
