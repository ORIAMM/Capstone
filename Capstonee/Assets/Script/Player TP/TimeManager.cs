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
    public float SkillCD;
    public float CurrSkillCD;
    public bool CoolDown;

    bool skillCooldown => Time.time >= skillTime;
    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        CurrSkillCD = SkillCD;
        CoolDown = false;
    }
    private void Update()
    {
        if (CoolDown)
        {
            CurrSkillCD -= Time.deltaTime;
        }
        if (CurrSkillCD < 0)
        {
            CoolDown = false;
        }
    }
    private Coroutine coroutine;
    public void UseSkill(float time) => coroutine ??= StartCoroutine(StopTime(time));
    public IEnumerator StopTime(float Timer)
    {
        if (isStopped == false && skillCooldown && !CoolDown)
        {
            skillTime = Time.time - skillCoolTime;
            Debug.Log("STOPPPP");
            isStopped = true;
            animator.SetTrigger("Skill");
            yield return new WaitForSeconds(Timer);

            Debug.Log("Jalan");
            isStopped = false;
            coroutine = null;
            CoolDown = true;
            CurrSkillCD = SkillCD;
        }
    }
}
