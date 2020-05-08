using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    // Editor Fields
    public ARSessionOrigin sessionOrigin;
    public GameObject bottle;
    public GameObject note;
    public GameObject block;

    [SerializeField] private Button btnRestart = null; // assign in the editor

    private enum State
    {
        Block = 0,
        Bottle = 1,
        Note = 2
    }
    private State state;

    void Start()
    {
        // Setup the visibility of objects
        block.SetActive(true);
        bottle.SetActive(false);
        note.SetActive(false);

        // Set the initial state of the app
        state = State.Block;

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
                if (state == State.Block)
                {
                    this.state = State.Bottle;
                    block.SetActive(false);
                    bottle.SetActive(true);
                }
                else if (state == State.Bottle)
                {
                    this.state = State.Note;
                    bottle.SetActive(false);
                    note.SetActive(true);
                }
                else
                {
                    this.state = State.Block;
                    note.SetActive(false);
                    block.SetActive(true);
                }
            }
        }
    }

}
