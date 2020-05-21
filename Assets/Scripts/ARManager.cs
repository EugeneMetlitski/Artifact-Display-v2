using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARManager : MonoBehaviour
{
    // Editor Fields
    public float dragSpeedX = 0.001f;
    public float dragSpeedY = 0.0001f;
    public float minDragDistance = 5.0f;
    public ARSessionOrigin sessionOrigin;
    public GameObject screenContent;
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
    public Text ans_6;

    // Private Fields
    private ReportData reportData;
    private Vector3 mouseNewPos;
    private Vector3 mousePrevPos;
    private enum MouseState { Up = 0, Down = 1, Dragged = 2, BottleDragged = 3, NoteDragged = 4 }
    private MouseState mouseState;
    private enum GameState { Active = 0, Paused = 1, JustUnpaused = 2 }
    private GameState gameState;
    private int secondsBeforeReset;
    private bool startResetCountdown;
    private bool restartResetCountdown;

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
        startResetCountdown = false;
        restartResetCountdown = false;

        // Instantiate and load the report data from file
        reportData = new ReportData(ans_1, ans_2, ans_3, ans_4, ans_5, ans_6);
        // Get the AutoReset time
        secondsBeforeReset = reportData.GetResetTime();

        // Prevent screen from going to sleep
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        // Figure out if application should be reset
        if (startResetCountdown) AutoResetApplication();

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
                    // Increase the # times block was clicked for report
                    reportData.SetNumBlockClicks(1);
                    ActivateAutoReset();
                }
                else if (hit.transform.name == "Bottle")
                {
                    noteContainer.SetActive(true);
                    bottle.GetComponent<Distructible>().Destroy();
                    // Increase the # times bottle was clicked for report
                    reportData.SetNumBottleClicks(1);
                    ActivateAutoReset();
                }
                else if (hit.transform.name == "Note-Object")
                {
                    note.GetComponent<Rotate>().RotateAround();
                    // Increase the # times note was clicked for report
                    reportData.SetNumNoteClicks(1);
                    ActivateAutoReset();
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
                ActivateAutoReset();
            }
            // If the note is dragged
            else if (mouseState == MouseState.NoteDragged)
            {
                noteContainer.transform.position = new Vector3(
                    noteContainer.transform.position.x + (mouseNewPos.x - mousePrevPos.x) * dragSpeedX,
                    noteContainer.transform.position.y,
                    noteContainer.transform.position.z + (mouseNewPos.y - mousePrevPos.y) * dragSpeedY
                );
                ActivateAutoReset();
            }
            // Update the previous mouse position
            if (mouseState != MouseState.Down)
                mousePrevPos = mouseNewPos;
        }
    }

    void OnApplicationQuit()
    {
        reportData.SaveData();
    }

    // Close the application
    public void CloseApplication()
    {
        reportData.SaveData();
        Application.Quit();
    }

    // Pause/unpause application based on boolean provided (true for pause, false for unpause)
    public void PauseApplicaiton(bool paused)
    {
        // Set game state based on boolean value provided, true for pause, false for unpause
        gameState = paused ? GameState.Paused : GameState.JustUnpaused;
    }

    // Reset the application
    public void ResetApplication()
    {
        reportData.SaveData();
        SceneManager.LoadScene("Image_Recognition");
    }

    // Public function that lets other objects trigger reset countdown
    public void ActivateAutoReset()
    {
        startResetCountdown = true;
        restartResetCountdown = true;
    }

    // Reset all the values in the usage report
    public void ResetUsageReport()
    {
        ActivateAutoReset();
        reportData.ResetUsageReport();
    }

    // Change the time it takes for app to AutoReset
    public void ChangeResetTime(int incrementAmount)
    {
        ActivateAutoReset();
        reportData.SetResetTime(incrementAmount);
        secondsBeforeReset = reportData.GetResetTime();
    }

    // Update the Usage Report with number of times screen was clicked
    public void RecordScreenClicked()
    {
        // Increase the # times note was clicked for report
        reportData.SetNumScreenClicks(1);
        ActivateAutoReset();
    }

    // Figure out if application should be reset
    private void AutoResetApplication()
    {
        if (restartResetCountdown)
        {
            restartResetCountdown = false;
            CancelInvoke();
        }
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

    private class ReportData
    {
        private int numSessions;
        private int numClicksBlock;
        private int numClicksBottle;
        private int numClicksNote;
        private int numClicksScreen;
        private int resetTime;

        private readonly Text[] ans;

        public ReportData(
            Text ans_1, Text ans_2, Text ans_3,
            Text ans_4, Text ans_5, Text ans_6)
        {
            ans = new Text[6];
            ans[0] = ans_1;
            ans[1] = ans_2;
            ans[2] = ans_3;
            ans[3] = ans_4;
            ans[4] = ans_5;
            ans[5] = ans_6;

            LoadData();
        }

        public void SetNumSessions(int incrementAmount)
        {
            numSessions += incrementAmount;
            ans[0].text = numSessions.ToString();
        }

        public void SetNumBlockClicks(int incrementAmount)
        {
            numClicksBlock += incrementAmount;
            ans[1].text = numClicksBlock.ToString() + " times";
        }

        public void SetNumBottleClicks(int incrementAmount)
        {
            numClicksBottle += incrementAmount;
            ans[2].text = numClicksBottle.ToString() + " times";
        }

        public void SetNumNoteClicks(int incrementAmount)
        {
            numClicksNote += incrementAmount;
            ans[3].text = numClicksNote.ToString() + " times";
        }

        public void SetNumScreenClicks(int incrementAmount)
        {
            numClicksScreen += incrementAmount;
            ans[4].text = numClicksScreen.ToString() + " times";
        }

        public void SetResetTime(int incrementAmount)
        {
            resetTime += incrementAmount;

            // Revert incrementation if result value exceeds bounds
            if (resetTime < 10 || resetTime >= 900)
                resetTime -= incrementAmount;

            // Update the text field
            ans[5].text = resetTime.ToString() + " seconds";
        }

        public int GetResetTime()
        {
            return resetTime;
        }

        private void UpdateAllFields()
        {
            SetNumSessions(0);
            SetNumBlockClicks(0);
            SetNumBottleClicks(0);
            SetNumNoteClicks(0);
            SetNumScreenClicks(0);
            SetResetTime(0);
        }

        // Save the usage report data to file
        public void SaveData()
        {
            // Save to file (using player prefs unity function)
            PlayerPrefs.SetInt("ans_1", numSessions);
            PlayerPrefs.SetInt("ans_2", numClicksBlock);
            PlayerPrefs.SetInt("ans_3", numClicksBottle);
            PlayerPrefs.SetInt("ans_4", numClicksNote);
            PlayerPrefs.SetInt("ans_5", numClicksScreen);
            PlayerPrefs.SetInt("ans_6", resetTime);
            PlayerPrefs.Save();
        }

        // Load report data from file
        private void LoadData()
        {
            if (PlayerPrefs.HasKey("ans_1")) // load # of sessions
                numSessions = PlayerPrefs.GetInt("ans_1") + 1;
            else numSessions = 1;

            if (PlayerPrefs.HasKey("ans_2")) // load # of times block clicked
                numClicksBlock = PlayerPrefs.GetInt("ans_2");
            else numClicksBlock = 0;

            if (PlayerPrefs.HasKey("ans_3")) // load # of times bottle clicked
                numClicksBottle = PlayerPrefs.GetInt("ans_3");
            else numClicksBottle = 0;

            if (PlayerPrefs.HasKey("ans_4")) // load # of times note clicked
                numClicksNote = PlayerPrefs.GetInt("ans_4");
            else numClicksNote = 0;

            if (PlayerPrefs.HasKey("ans_5")) // load # of times screen clicked
                numClicksScreen = PlayerPrefs.GetInt("ans_5");
            else numClicksScreen = 0;

            if (PlayerPrefs.HasKey("ans_6")) // load reset time
                resetTime = PlayerPrefs.GetInt("ans_6");
            else resetTime = 120;

            UpdateAllFields();
        }

        // Reset the usage report to initial values
        public void ResetUsageReport()
        {
            // Reset all the data
            numSessions = 1;
            numClicksBlock = 0;
            numClicksBottle = 0;
            numClicksNote = 0;
            numClicksScreen = 0;

            // Update the text fields
            UpdateAllFields();
        }
    }
}
