using System.Collections;
using UnityEngine;

public abstract class TimedObject : MonoBehaviour
{
    public Coroutine TimeStopped;
    protected virtual void Update()
    {
        if (TimeManager.instance.isStopped || TimeStopped != null) TimeStopped ??= StartCoroutine(stopLogic());
    }
    public IEnumerator stopLogic()
    {
        OnStop();
        yield return new WaitUntil(() => TimeManager.instance.isStopped == false);
        OnContinue();
        TimeStopped = null;
    }
    public abstract void OnStop();
    public abstract void OnContinue();
}
