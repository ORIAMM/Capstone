using UnityEngine;

public class SoundObject : TimedObject
{
    [SerializeField] AudioSource aud;
    [SerializeField] string SFX_Name;

    public override void OnContinue()
    {
        aud.UnPause();
    }

    public override void OnStop()
    {
        aud.Pause();
    }

    public override void OnUpdate() { }

    private void Awake()
    {
        //aud.outputAudioMixerGroup = SoundManager.instance;
    }
    private void OnEnable()
    {
        SoundManager.instance.PlaySFX(SFX_Name);
        aud.Play();
        PoolManager.ReleaseObject(gameObject);
    }
}
