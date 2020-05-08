using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    // Editor Fields
    public ARSessionOrigin sessionOrigin;
    public GameObject bottle;
    public GameObject note;
    public GameObject block;
    public GameObject btnReset;

    void Start()
    {
        // Setup the visibility of objects
        block.SetActive(true);
        bottle.SetActive(false);
        note.SetActive(false);

        //Debug.Log("Program Started");
    }

    void Update()
    {
        // If any part of the screen was clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Send a ray to figure out if any object was hit by the ray
            Ray ray = sessionOrigin.camera.ScreenPointToRay(Input.mousePosition);

            // If the ray collided with an object
            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                if (hit.transform.name == "Block")
                {
                    block.SetActive(false);
                    bottle.SetActive(true);
                }
                else if (hit.transform.name == "Bottle")
                {
                    bottle.SetActive(false);
                    note.SetActive(true);
                }
                else if (hit.transform.name == "Note")
                {
                    Debug.Log("Note clicked.");
                }
            }
        }
    }
}
