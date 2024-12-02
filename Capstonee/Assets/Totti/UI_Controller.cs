using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels;
    private Stack<GameObject> panelHistory = new Stack<GameObject>();
    private GameObject currentPanel;

    public GameObject pauseMenuUI;
    private bool isPaused;
    void Start()
    {
        isPaused = false;

        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        if (panels.Count > 0)
        {
            currentPanel = panels[0];
            currentPanel.SetActive(true);
        }
    }
    public void OpenPanel(GameObject newPanel)
    {
        if (currentPanel != null)
        {
            panelHistory.Push(currentPanel);
            currentPanel.SetActive(false);
        }

        currentPanel = newPanel;
        currentPanel.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Back()
    {
        if (panelHistory.Count > 0)
        {
            currentPanel.SetActive(false);
            currentPanel = panelHistory.Pop();
            currentPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Tidak ada panel sebelumnya.");
        }
    }
    public void ChangeScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void PauseGame()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);
            }
        }

        else if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }
        }
    }
}
