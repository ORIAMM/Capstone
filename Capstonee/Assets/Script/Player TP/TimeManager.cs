using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using Vignette = UnityEngine.Rendering.Universal.Vignette;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [HideInInspector] public bool isStopped;
    [SerializeField] private Animator animator;
    [SerializeField] private float skillCoolTime;
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;
    private Vignette Vignettes;

    [Header("SoundClip")]
    [SerializeField] private SoundCLIP StartSkill;

    float skillTime = 0;
    public bool Skillready => Time.time >= skillTime;
    private void Awake()
    {
        instance = this;
        if (globalVolume != null && globalVolume.profile.TryGet<ColorAdjustments>(out var adjustments))
        {
            colorAdjustments = adjustments;
        }
        if (globalVolume != null && globalVolume.profile.TryGet<Vignette>(out var Vig))
        {
            Vignettes = Vig;
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
            SoundCEO.instance.PlaySound(StartSkill);

            if (colorAdjustments != null)
            {
                yield return new WaitForSeconds(0.5f);
                colorAdjustments.saturation.value = -100f; // Ubah saturation menjadi hitam putih
                Vignettes.rounded.overrideState = true; 
                Vignettes.rounded.value = true;
            }

            yield return new WaitForSeconds(Timer);

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = 0f; // Normal saturation
                Vignettes.rounded.overrideState = true;
                Vignettes.rounded.value = false;
            }

            Debug.Log("Jalan");
            isStopped = false;
        }
        OnCooldown = null;
    }
}
