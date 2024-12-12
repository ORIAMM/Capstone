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
public class VideoPlayerHandler : MonoBehaviour, ICommand
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
        Invoker.ExecuteCommand(this);
    }
    private void ChangeScene(VideoPlayer vp)
    {
        vp.loopPointReached -= ChangeScene; 
        GameManager.ChangeScene(sceneName); 
    }

    public void Execute()
    {
        timer = new(1, () =>
        {
            videoPlayer.Play();
            Canvas.SetActive(false);
        });
        videoPlayer.loopPointReached += ChangeScene;
    }

    public void Undo()
    {
        Debug.Log(videoPlayer.frameCount - 1);
        videoPlayer.frame = (long)videoPlayer.frameCount - 1;
    }
}
