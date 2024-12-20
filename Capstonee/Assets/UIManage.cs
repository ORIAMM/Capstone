using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEditor;

public class UIManage : MonoBehaviour
{
    public static UIManage instance;

    [SerializeField] private PlayerInputManager PIM;
    [SerializeField] private GameObject Back;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject result;

    public int Deathcount;

    private void Awake()
    {
        if (instance == null) instance = this;
        //PIM = FindObjectOfType<PlayerInputManager>();
    }

    public void DeathCheck()
    {
        Debug.LogWarning(Deathcount);
        if (Deathcount >= PIM.playerCount)
        {
            Back.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0.1f;
        }
        else Debug.LogWarning("Not Done");
    }
    public void Win()
    {
        StartCoroutine(Result());
    }

    public void BackScene()
    {
        SoundCEO.instance.StopAllSound();
        ChangeScene("Menu");
    }

    public void ChangeScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
    public void Exit()
    {
        Application.Quit();
    }
    IEnumerator Result()
    {
        Time.timeScale = 0.1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        WinPanel.SetActive(true);
        var CGVictory = WinPanel.GetComponent<CanvasGroup>();
        CGVictory.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);  

        result.SetActive(true);
        var CGResult = result.GetComponent<CanvasGroup>();
        CGResult.DOFade(1, 0.5f);
        
        //SoundManager.instance.PlaySFX("Victory");

    }


}
