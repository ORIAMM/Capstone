using UnityEngine;

public class SoundObject : TimedObject
{
    [SerializeField] string SFX_Name;
    [SerializeField] private SoundCLIP soundClip;

    public override void OnContinue()
    {
        SoundCEO.instance.ResumeClip(soundClip);
    }

    public override void OnStop()
    {
        SoundCEO.instance.PauseClip(soundClip);
    }

    public override void OnUpdate() { }

    private void Awake()
    {
        //aud.outputAudioMixerGroup = SoundManager.instance;
    }
    private void OnEnable()
    {
        //SoundManager.instance.PlaySFX(SFX_Name);
        SoundCEO.instance.PlaySound(soundClip);
        PoolManager.ReleaseObject(gameObject);
    }
}
