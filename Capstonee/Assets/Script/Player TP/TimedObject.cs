using System;
using System.Collections;
using UnityEngine;

public abstract class TimedObject : MonoBehaviour
{
    protected bool isPaused;
    protected Coroutine TimeStopped;
    protected virtual void Update()
    {
        if (TimeManager.instance.isStopped || TimeStopped != null)
        {
            TimeStopped ??= StartCoroutine(StopLogic());
            return;
        }
        OnUpdate(); 
    }
    public abstract void OnUpdate();

    public IEnumerator StopLogic()
    {
        isPaused = true;
        OnStop();
        yield return new WaitUntil(() => TimeManager.instance.isStopped == false);
        OnContinue();
        isPaused = false;
        TimeStopped = null;
    }
    public abstract void OnStop();
    public abstract void OnContinue();
}
