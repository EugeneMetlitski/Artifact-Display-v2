using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    // Editor Fields
    public ARSessionOrigin sessionOrigin;
    public GameObject bottle;
    public GameObject note;
    public GameObject block;

    private bool isbBlockVisible;
    private bool isBottleVisible;
    private bool isNoteVisible;

    void Start()
    {
        // Setup the visibility of objects
        isbBlockVisible = true;
        isBottleVisible = true;
        isNoteVisible = false;
        block.SetActive(isbBlockVisible);
        bottle.SetActive(isBottleVisible);
        note.SetActive(isNoteVisible);

        // This debug message appears in console window
        // when play is clicked in Unity
        //Debug.Log("Program Started");
    }

    void Update()
    {
        CheckIfObjectClicked();
    }

    private void CheckIfObjectClicked()
    {
        // If any part of the screen was clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Send a ray to figure out if any object was hit by the ray
            Ray ray = sessionOrigin.camera.ScreenPointToRay(Input.mousePosition);
            bool collision = Physics.Raycast(ray, out _, 10);

            // If any of the object (with a collision script) has been clicked
            if (collision)
            {
                isNoteVisible = !isNoteVisible;
                note.SetActive(isNoteVisible);
            }
        }
    }

}
