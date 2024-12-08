using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [HideInInspector] public bool isStopped;
    public Animator animator;
    [SerializeField] private float skillCoolTime;
    float skillTime = 0;
    public PlayerTP playerTP;
    public bool skillCooldown => Time.time >= skillTime;
    private void Awake()
    {
        instance = this;
        playerTP = FindObjectOfType<PlayerTP>().GetComponent<PlayerTP>();
    }

    private Coroutine coroutine;

    public void UseSkill(float time) => coroutine ??= StartCoroutine(StopTime(time));

    public IEnumerator StopTime(float Timer)
    {
        if (isStopped == false && skillCooldown)
        {
            
            Debug.Log("STOPPPP");
            playerTP.HealthPlayer -= 50f;
            isStopped = true;
            animator.SetTrigger("Used");
            yield return new WaitForSeconds(Timer);

            Debug.Log("Jalan");
            isStopped = false;
            coroutine = null;
            skillTime = Time.time - skillCoolTime;
        }
    }
}
