using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

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

    [Header("Post Processing")]
    [SerializeField] private Volume globalVolume; // Referensi ke Global Volume
    private ColorAdjustments colorAdjustments; // Untuk mengontrol efek Color Adjustments

    bool skillCooldown => Time.time >= skillTime;
    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
        if (globalVolume != null && globalVolume.profile.TryGet<ColorAdjustments>(out var adjustments))
        {
            colorAdjustments = adjustments;
        }
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

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = -100f; // Ubah saturation menjadi hitam putih
            }
            SoundManager.instance.PlaySFX("TimeStop");

            yield return new WaitForSeconds(Timer);

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = 0f; // Normal saturation
            }

            Debug.Log("Jalan");
            isStopped = false;
            coroutine = null;
            CoolDown = true;
            CurrSkillCD = SkillCD;
        }
    }
}
