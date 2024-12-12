using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [HideInInspector] public bool isStopped;
    [SerializeField] private Animator animator;
    [SerializeField] private float skillCoolTime;
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    float skillTime = 0;
    public bool Skillready => Time.time >= skillTime;
    private void Awake()
    {
        instance = this;
        if (globalVolume != null && globalVolume.profile.TryGet<ColorAdjustments>(out var adjustments))
        {
            colorAdjustments = adjustments;
        }
    }

    public Coroutine OnCooldown;

    public void UseSkill(float time) => OnCooldown ??= StartCoroutine(StopTime(time));

    public IEnumerator StopTime(float Timer)
    {
        Debug.Log(Skillready);
        if (isStopped == false && Skillready)
        {
            skillTime = Time.time + skillCoolTime;
            Debug.Log(skillTime);
            Debug.Log("STOPPPP");
            isStopped = true;
            animator.Play("Cooldown_anim");

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = -100f; // Ubah saturation menjadi hitam putih
            }

            yield return new WaitForSeconds(Timer);

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = 0f; // Normal saturation
            }
            Debug.Log("Jalan");
            isStopped = false;
        }
        OnCooldown = null;
    }
}
