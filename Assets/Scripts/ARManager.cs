using System;
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
    public GameObject menu;
    public GameObject menuButton;
    public float dragSpeedX = 0.001f;
    public float dragSpeedY = 0.0001f;
    public float minDragDistance = 5.0f;

    // Private Fields
    private Vector3 mouseNewPos;
    private Vector3 mousePrevPos;
    private enum MouseState { Up = 0, Down = 1, Dragged = 2, BottleDragged = 3, NoteDragged = 4 }
    private MouseState mouseState;
    private enum GameState { Active = 0, Paused = 1, JustUnpaused = 2 }
    private GameState gameState;

void Start()
    {
        // Setup the visibility of objects
        block.SetActive(true);
        bottle.SetActive(false);
        noteContainer.SetActive(false);
        menu.SetActive(false);
        menuButton.SetActive(true);
        mouseState = MouseState.Up;
        gameState = GameState.Active;

        //Debug.Log("Program Started");
    }

    void Update()
    {
        // Don't update anything if applicatoins is paused
        if (gameState == GameState.Paused)
            return;
        // If the game was just unpaused, set gamestate to active and exit this function
        else if (gameState == GameState.JustUnpaused)
        {
            gameState = GameState.Active;
            return;
        }

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
                // Calculate the distance the mouse has travelled from when mouse was first down
                float mouseMovedDistance = Vector3.Distance(mouseNewPos, mousePrevPos);

                // If the mouse has moved significant distance to start dragging and object
                if (mouseMovedDistance > minDragDistance)
                {
                    mouseState = MouseState.Dragged; // Indicate that the mouse is potentially dragging object

                    // Figure out if the mouse has hit any object when it started dragging
                    Ray ray = sessionOrigin.camera.ScreenPointToRay(mousePrevPos);
                    if (Physics.Raycast(ray, out RaycastHit hit, 10))
                    {
                        if (hit.transform.name == "Bottle")
                            mouseState = MouseState.BottleDragged;
                        else if (hit.transform.name == "Note-Object")
                            mouseState = MouseState.NoteDragged;
                    }
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
            if (mouseState != MouseState.Down)
                mousePrevPos = mouseNewPos;
        }
    }

    public void PauseApplicaiton(bool paused)
    {
        // Set game state based on boolean value provided, true for pause, false for unpause
        gameState = paused ? GameState.Paused : GameState.JustUnpaused;
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
            // If the position of the mouse has changed by significant amount, return true
            return (mouseNewPos != mousePrevPos) ? true : false;
        }
        else
        {
            // If mouse has not been clicked down, return false
            return false;
        }
    }
}
