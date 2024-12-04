using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [HideInInspector] public bool isStopped;
    [HideInInspector] public Animator animator;
    private void Awake()
    {
        instance = this;
    }

    private Coroutine coroutine;

    public void UseSkill(float time) => coroutine ??= StartCoroutine(StopTime(time));

    public IEnumerator StopTime(float Time)
    {
        if (isStopped == false)
        {
            Debug.Log("STOPPPP");
            isStopped = true;
            animator.SetTrigger("Skill");
            yield return new WaitForSeconds(Time);

            Debug.Log("Jalan");
            isStopped = false;
            coroutine = null;
        }
    }
}
