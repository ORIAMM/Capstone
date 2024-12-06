using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.Windows;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels;
    private Stack<GameObject> panelHistory = new Stack<GameObject>();
    private GameObject currentPanel;
    private string currentScene;

    public GameObject pauseMenuUI;
    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public GameObject ResultPanel;

    private bool isPaused;
    private string currentMusic;
    public bool isWin;
    public bool isLose;

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneAfterVideo;
    void Start()
    {
        Time.timeScale = 1f;

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
        SoundManager.instance.StopAllSFX();
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

                Cursor.visible = false;

                isLose = false;
                isWin = false;
                break;

            default:
                //.LogWarning("No audio clip assigned for this scene.");
                break;
        }
    }
    public float TimeElapsed;
    private void Update()
    {
        TimeElapsed += Time.deltaTime;
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
    //public void PauseGame()
    //{
    //    if (!isPaused)
    //    {
    //        Time.timeScale = 0f;
    //        isPaused = true;
    //        //if (pauseMenuUI != null && is)
    //        //{
    //        //    pauseMenuUI.SetActive(true);
    //        //}
    //        SoundManager.instance.PauseMusic();
    //        SoundManager.instance.PauseSfx();
            
    //    }

    //    else if (isPaused)
    //    {
    //        Time.timeScale = 1f;
    //        isPaused = false;
    //        //if (pauseMenuUI != null)
    //        //{
    //        //    pauseMenuUI.SetActive(false);
    //        //}
    //        SoundManager.instance.UnPauseMusic();
    //        SoundManager.instance.UnPauseSfx();
    //    }
    //}
    public void Victory()
    {
        if (isWin)
        {
            Cursor.visible = true;
            VictoryPanel.SetActive(true);
            Invoke("Result", 1f);
        }
    }
    public void Defeat()
    {
        if (isLose)
        {
            Cursor.visible = true;
            DefeatPanel.SetActive(true);
            Time.timeScale = 0.1f;

            SoundManager.instance.PlaySFX("PlayerDeath");
        }
    }

    public CD_UI cd;
    public Player player;
    public TextMeshProUGUI _timeremain;
    public TextMeshProUGUI _timeelapsed;
    public void Result()
    {
        VictoryPanel.SetActive(false);
        Time.timeScale = 0.1f;
        ResultPanel.SetActive(true);
        SoundManager.instance.PlaySFX("Victory");

        FloatToTimeRemain();
        FloatToTimeElapsed();
    }
    public void FloatToTimeRemain()
    {
        int totalSeconds = Mathf.CeilToInt(player.HealthPlayer); // Pembulatan ke atas
        int minutes = totalSeconds / 60; // Hitung menit
        int seconds = totalSeconds % 60; // Hitung detik

        _timeremain.text = $"{minutes:00}:{seconds:00}"; // Format MM:SS
    }
    public void FloatToTimeElapsed()
    {
        int totalSeconds = Mathf.CeilToInt(TimeElapsed); // Pembulatan ke atas
        int minutes = totalSeconds / 60; // Hitung menit
        int seconds = totalSeconds % 60; // Hitung detik

        _timeelapsed.text = $"{minutes:00}:{seconds:00}"; // Format MM:SS
    }

    public void DelayVideo()
    {
        ButtonClicked();
        Invoke("PlayGameWithCutscene", 1f);
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
            //Debug.LogWarning("VideoPlayer tidak diset. Langsung berpindah scene.");
            ChangeScene(nextSceneAfterVideo);
        }
    }
    private void OnVideoFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnVideoFinished; // Hapus callback
        ChangeScene(nextSceneAfterVideo); // Ganti ke scene berikutnya
    }
}
