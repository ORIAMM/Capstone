using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
            Time.timeScale = 0.1f;
        }
        else Debug.LogWarning("Not Done");
    }
    public void Win()
    {
        StartCoroutine(Result());
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

        WinPanel.SetActive(true);
        var CGVictory = WinPanel.GetComponent<CanvasGroup>();
        CGVictory.DOFade(1, 0.5f);
        yield return null;  

        result.SetActive(true);
        var CGResult = result.GetComponent<CanvasGroup>();
        CGResult.DOFade(1, 0.5f);
        
        //SoundManager.instance.PlaySFX("Victory");

    }


}
