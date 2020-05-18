using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject aRSession;
    public GameObject menu;
    public GameObject menuButton;
    public GameObject usageReport;

    public void OnMenuButtonClicked()
    {
        aRSession.GetComponent<ARManager>().PauseApplicaiton(true);
        usageReport.SetActive(false);
        menuButton.SetActive(false);
        menu.SetActive(true);
    }

    public void OnResetAppClicked()
    {
        aRSession.GetComponent<ARManager>().ResetApplication();
    }

    public void OnUsageReportClicked()
    {
        menu.SetActive(false);
        usageReport.SetActive(true);
    }

    public void OnBackToAppClicked()
    {
        menu.SetActive(false);
        usageReport.SetActive(false);
        menuButton.SetActive(true);
        aRSession.GetComponent<ARManager>().PauseApplicaiton(false);
    }

    public void OnCloseAppClicked()
    {
        aRSession.GetComponent<ARManager>().CloseApplication();
    }

    public void OnResetCountsClicked()
    {
        aRSession.GetComponent<ARManager>().ResetUsageReport();
    }
}
