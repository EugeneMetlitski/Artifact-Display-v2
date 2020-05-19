using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainContainer;
    public GameObject menu;
    public GameObject menuButton;
    public GameObject usageReport;

    public void OnMenuButtonClicked()
    {
        mainContainer.GetComponent<ARManager>().PauseApplicaiton(true);
        usageReport.SetActive(false);
        menuButton.SetActive(false);
        menu.SetActive(true);
    }

    public void OnResetAppClicked()
    {
        mainContainer.GetComponent<ARManager>().ResetApplication();
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
        mainContainer.GetComponent<ARManager>().PauseApplicaiton(false);
    }

    public void OnCloseAppClicked()
    {
        mainContainer.GetComponent<ARManager>().CloseApplication();
    }

    public void OnResetCountsClicked()
    {
        mainContainer.GetComponent<ARManager>().ResetUsageReport();
    }
}
