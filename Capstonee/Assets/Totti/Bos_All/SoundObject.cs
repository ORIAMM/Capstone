using UnityEngine;

public class SoundObject : TimedObject
{
    [SerializeField] string SFX_Name;

    public override void OnContinue()
    {
    }

    public override void OnStop()
    {
    }

    public override void OnUpdate() { }

    private void Awake()
    {
        //aud.outputAudioMixerGroup = SoundManager.instance;
    }
    private void OnEnable()
    {
        SoundManager.instance.PlaySFX(SFX_Name);
        PoolManager.ReleaseObject(gameObject);
    }
}
