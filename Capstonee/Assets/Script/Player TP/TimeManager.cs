using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [HideInInspector] public bool isStopped;
    [HideInInspector] public Animator animator;
    [SerializeField] private float skillCoolTime;
    float skillTime = 0;
    bool skillCooldown => Time.time >= skillTime;
    private void Awake()
    {
        instance = this;
    }

    private Coroutine coroutine;

    public void UseSkill(float time) => coroutine ??= StartCoroutine(StopTime(time));

    public IEnumerator StopTime(float Timer)
    {
        if (isStopped == false && skillCooldown)
        {
            skillTime = Time.time - skillCoolTime;
            Debug.Log("STOPPPP");
            isStopped = true;
            animator.SetTrigger("Skill");
            yield return new WaitForSeconds(Timer);

            Debug.Log("Jalan");
            isStopped = false;
            coroutine = null;
        }
    }
}
