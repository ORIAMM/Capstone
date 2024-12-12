using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [HideInInspector] public bool isStopped;
    [SerializeField] private Animator animator;
    [SerializeField] private float skillCoolTime;
    float skillTime = 0;
    bool Skillready => Time.time >= skillTime;
    private void Awake()
    {
        instance = this;
    }

    public Coroutine OnCooldown;

    public void UseSkill(float time) => OnCooldown ??= StartCoroutine(StopTime(time));

    public IEnumerator StopTime(float Timer)
    {
        if (isStopped == false && Skillready)
        {
            skillTime = Time.time + skillCoolTime;
            Debug.Log(skillTime);
            Debug.Log("STOPPPP");
            isStopped = true;
            animator.Play("Cooldown_anim");
            yield return new WaitForSeconds(Timer);
            Debug.Log("Jalan");
            isStopped = false;
        }
        OnCooldown = null;
    }
}
