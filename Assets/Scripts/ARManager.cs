using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARManager : MonoBehaviour
{
    // Editor Fields
    public int secondsBeforeReset = 120;
    public float dragSpeedX = 0.001f;
    public float dragSpeedY = 0.0001f;
    public float minDragDistance = 5.0f;
    public ARSessionOrigin sessionOrigin;
    public GameObject block;
    public GameObject bottle;
    public GameObject noteContainer;
    public GameObject note;
    public GameObject menu;
    public GameObject menuButton;
    public GameObject usageReport;
    public Text ans_1;
    public Text ans_2;
    public Text ans_3;
    public Text ans_4;
    public Text ans_5;

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
        usageReport.SetActive(false);
        menuButton.SetActive(true);

        // Set the states of mouse and game/application
        mouseState = MouseState.Up;
        gameState = GameState.Active;

        // Load the report data from file
        LoadReportData();

        // Increase the number of total sessions in report
        ans_1.text = (Int32.Parse(ans_1.text) + 1).ToString();
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

        // Figure out if application should be reset
        AutoResetApplication();

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
                    // Increase the # times block was clicked for report
                    ans_2.text = (Int32.Parse(ans_2.text) + 1).ToString();
                }
                else if (hit.transform.name == "Bottle")
                {
                    noteContainer.SetActive(true);
                    bottle.GetComponent<Distructible>().Destroy();
                    // Increase the # times bottle was clicked for report
                    ans_3.text = (Int32.Parse(ans_3.text) + 1).ToString();
                }
                else if (hit.transform.name == "Note-Object")
                {
                    note.GetComponent<Rotate>().RotateAround();
                    // Increase the # times note was clicked for report
                    ans_4.text = (Int32.Parse(ans_4.text) + 1).ToString();
                }
                else if (hit.transform.name == "Screen")
                {
                    // Increase the # times note was clicked for report
                    ans_5.text = (Int32.Parse(ans_5.text) + 1).ToString();
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

    // Pause/unpause application based on boolean provided (true for pause, false for unpause)
    public void PauseApplicaiton(bool paused)
    {
        // Set game state based on boolean value provided, true for pause, false for unpause
        gameState = paused ? GameState.Paused : GameState.JustUnpaused;
    }

    // Close the application
    public void CloseApplication()
    {
        SaveReportData();
        Application.Quit();
    }

    // Reset the application
    public void ResetUsageReport()
    {
        // Set the data to text fields
        ans_1.text = "0";
        ans_2.text = "0";
        ans_3.text = "0";
        ans_4.text = "0";
        ans_5.text = "0";
    }

    // Reset the application
    public void ResetApplication()
    {
        SaveReportData();
    }

    // Figure out if application should be reset
    private void AutoResetApplication()
    {
        if (Input.anyKeyDown)
            CancelInvoke();
        else
            Invoke("ResetApplication", secondsBeforeReset);
    }

    // If the mouse has been clicked down and dragged, returns true
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

    // Load the usage report data from file
    private void LoadReportData()
    {
        // Set up initial values for usage report
        ResetUsageReport();

        if (PlayerPrefs.HasKey("ans_1")) // load # of sessions
            ans_1.text = PlayerPrefs.GetInt("ans_1").ToString();
        if (PlayerPrefs.HasKey("ans_2")) // load # of times block clicked
            ans_2.text = PlayerPrefs.GetInt("ans_2").ToString();
        if (PlayerPrefs.HasKey("ans_3")) // load # of times bottle clicked
            ans_3.text = PlayerPrefs.GetInt("ans_3").ToString();
        if (PlayerPrefs.HasKey("ans_4")) // load # of times note clicked
            ans_4.text = PlayerPrefs.GetInt("ans_4").ToString();
        if (PlayerPrefs.HasKey("ans_5")) // load # of times screen clicked
            ans_5.text = PlayerPrefs.GetInt("ans_5").ToString();
    }

    // Save the usage report data to file
    private void SaveReportData()
    {
        // Save to file (using player prefs unity function)
        PlayerPrefs.SetInt("ans_1", Int32.Parse(ans_1.text));
        PlayerPrefs.SetInt("ans_2", Int32.Parse(ans_2.text));
        PlayerPrefs.SetInt("ans_3", Int32.Parse(ans_3.text));
        PlayerPrefs.SetInt("ans_4", Int32.Parse(ans_4.text));
        PlayerPrefs.SetInt("ans_5", Int32.Parse(ans_5.text));
        PlayerPrefs.Save();
    }
}
