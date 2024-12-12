using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Timer
{
    public float waitTime;
    public Action onTime;
    public Timer(float waitTime, Action onTime) { 
        this.waitTime = waitTime;
        this.onTime = onTime;
    }
    public void TimerLogic()
    {
        waitTime -= Time.deltaTime;
        if (waitTime < 0) onTime?.Invoke();
    }
}
public class VideoPlayerHandler : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject Canvas;
    private VideoPlayer videoPlayer;
    private Timer timer;
    public void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }
    private void Update()
    {
        timer?.TimerLogic();
    }
    public void PlayVideo()
    {
        timer = new(1, () =>
        {
            Canvas.SetActive(false);
            videoPlayer.Play();
        });
        videoPlayer.loopPointReached += ChangeScene;
    }
    private void ChangeScene(VideoPlayer vp)
    {
        vp.loopPointReached -= ChangeScene; 
        GameManager.ChangeScene(sceneName); 
    }
}
