using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    // Editor Fields
    public ARSessionOrigin sessionOrigin;
    public GameObject block;
    public GameObject bottle;
    public GameObject noteContainer;
    public GameObject note;
    public GameObject btnReset;
    public float dragSpeedX = 0.001f;
    public float dragSpeedY = 0.01f;

    // Private Fields
    private Vector3 mouseNewPos;
    private Vector3 mousePrevPos;
    enum MouseState { Up = 0, Down = 1, Dragged = 2, BottleDragged = 3, NoteDragged = 4 }
    private MouseState mouseState;

    void Start()
    {
        // Setup the visibility of objects
        block.SetActive(true);
        bottle.SetActive(false);
        noteContainer.SetActive(false);
        mouseState = MouseState.Up;

        //Debug.Log("Program Started");
    }

    void Update()
    {
        // If click has been released on any part of the screen
        if (Input.GetMouseButtonUp(0))
        {
            // Send a ray to figure out if any object was hit by the ray
            Ray ray = sessionOrigin.camera.ScreenPointToRay(Input.mousePosition);

            // If the ray collided with an object and object is not being dragged
            if (Physics.Raycast(ray, out RaycastHit hit, 10) && mouseState == MouseState.Down)
            {
                if (hit.transform.name == "Block")
                {
                    bottle.SetActive(true);
                    block.GetComponent<Distructible>().Destroy();
                }
                else if (hit.transform.name == "Bottle")
                {
                    noteContainer.SetActive(true);
                    bottle.GetComponent<Distructible>().Destroy();
                }
                else if (hit.transform.name == "Note-Object")
                {
                    note.GetComponent<Rotate>().RotateAround();
                }
            }
            mouseState = MouseState.Up;
        }
        // If the mouse has been dragged
        else if (IsMouseDragged())
        {
            // If the dragging of the mouse has just started
            if (mouseState == MouseState.Down)
            {
                mouseState = MouseState.Dragged; // Indicate that the mouse is dragged

                // Figure out if the mouse has hit any object when it started dragging
                Ray ray = sessionOrigin.camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 10))
                {
                    if (hit.transform.name == "Bottle")
                        mouseState = MouseState.BottleDragged;
                    else if (hit.transform.name == "Note-Object")
                        mouseState = MouseState.NoteDragged;
                }
            }
            // If the bottle is dragged
            else if (mouseState == MouseState.BottleDragged)
            {
                bottle.transform.position = new Vector3(
                    bottle.transform.position.x + (mouseNewPos.x - mousePrevPos.x) * dragSpeedX,
                    bottle.transform.position.y,
                    bottle.transform.position.z + (mouseNewPos.y - mousePrevPos.y) * dragSpeedY
                );
            }
            // If the note is dragged
            else if (mouseState == MouseState.NoteDragged)
            {
                noteContainer.transform.position = new Vector3(
                    noteContainer.transform.position.x + (mouseNewPos.x - mousePrevPos.x) * dragSpeedX,
                    noteContainer.transform.position.y,
                    noteContainer.transform.position.z + (mouseNewPos.y - mousePrevPos.y) * dragSpeedY
                );
            }
            // Update the previous mouse position
            mousePrevPos = mouseNewPos;
        }
    }

    private bool IsMouseDragged()
    {
        // If the mouse has been pressed down
        if (Input.GetMouseButtonDown(0))
        {
            mouseState = MouseState.Down;
            mousePrevPos = Input.mousePosition;
            return false;
        }
        // If mouse is being held down
        else if (Input.GetMouseButton(0))
        {
            mouseNewPos = Input.mousePosition;
            // If the position of the mouse has changed return true
            return (mouseNewPos != mousePrevPos) ? true : false;
        }
        else
        {
            // If mouse has not been clicked down, return false
            return false;
        }
    }
}
