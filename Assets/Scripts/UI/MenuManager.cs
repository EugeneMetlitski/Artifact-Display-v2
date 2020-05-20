using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainContainer;
    public GameObject menu;
    public GameObject menuButton;
    public GameObject usageReport;

    public void OnMenuButtonClicked()
    {
        usageReport.SetActive(false);
        menuButton.SetActive(false);
        menu.SetActive(true);
        mainContainer.GetComponent<ARManager>().PauseApplicaiton(true);
        mainContainer.GetComponent<ARManager>().ActivateAutoReset();
    }

    public void OnResetAppClicked()
    {
        mainContainer.GetComponent<ARManager>().ResetApplication();
    }

    public void OnUsageReportClicked()
    {
        menu.SetActive(false);
        usageReport.SetActive(true);
        mainContainer.GetComponent<ARManager>().ActivateAutoReset();
    }

    public void OnBackToAppClicked()
    {
        menu.SetActive(false);
        usageReport.SetActive(false);
        menuButton.SetActive(true);
        mainContainer.GetComponent<ARManager>().PauseApplicaiton(false);
        mainContainer.GetComponent<ARManager>().ActivateAutoReset();
    }

    public void OnCloseAppClicked()
    {
        mainContainer.GetComponent<ARManager>().CloseApplication();
    }

    public void OnResetCountsClicked()
    {
        mainContainer.GetComponent<ARManager>().ResetUsageReport();
        mainContainer.GetComponent<ARManager>().ActivateAutoReset();
    }
}
