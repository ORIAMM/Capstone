using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels;
    private Stack<GameObject> panelHistory = new Stack<GameObject>();
    private GameObject currentPanel;
    private string currentScene;

    public GameObject pauseMenuUI;
    private bool isPaused;
    private string currentMusic;

    public GameObject _VictoryPanel;
    public GameObject _DefeatPanel;

    [SerializeField] private VideoPlayer videoPlayer; // Tambahkan referensi VideoPlayer
    [SerializeField] private string nextSceneAfterVideo; // Nama scene tujuan setelah video

    public InputAction Action;

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
        
        GetMusic();

    }
    public void GetMusic()
    {
        SoundManager.instance.StopAllMusic();
        currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "GameMenu":
                //Debug.Log("Main");
                currentMusic = "MainMenuTheme";
                SoundManager.instance.LoadPref();
                SoundManager.instance.PlayMusic("MainMenuTheme");
                break;

            case "BossFight":
                currentMusic = "BossFightTheme";
                SoundManager.instance.LoadPref();
                SoundManager.instance.PlayMusic("BossFightTheme");
                break;

            default:
                Debug.LogWarning("No audio clip assigned for this scene.");
                break;
        }
    }

    public void ButtonClicked()
    {
        SoundManager.instance.PlaySFX("UI_Click");
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
            SoundManager.instance.PauseMusic();
            SoundManager.instance.PauseSfx();
        }

        else if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }
            SoundManager.instance.UnPauseMusic();
            SoundManager.instance.UnPauseSfx();
        }
    }
    public void VictoryPanel()
    {
        _VictoryPanel.SetActive(true);
    }
    public void DefeatPanel()
    {
        _DefeatPanel.SetActive(true);
    }
    public void PlayGameWithCutscene()
    {
        if (videoPlayer != null)
        {
            SoundManager.instance.StopAllMusic();
            videoPlayer.loopPointReached += OnVideoFinished; // Callback saat video selesai
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("VideoPlayer tidak diset. Langsung berpindah scene.");
            ChangeScene(nextSceneAfterVideo);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnVideoFinished; // Hapus callback
        ChangeScene(nextSceneAfterVideo); // Ganti ke scene berikutnya
    }
}
