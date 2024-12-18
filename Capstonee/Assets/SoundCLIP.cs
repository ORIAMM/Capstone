using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioCategorys
{
    SFX,
    BGM,
    Master
}
[System.Serializable]
public class SoundCLIPArray
{
    public AudioClip[] clip;
}


[CreateAssetMenu(fileName = "NewAudioClip", menuName = "Audio/AudioClip")]
public class SoundCLIP : ScriptableObject
{
    public AudioClip[] clips;
    public bool loop;
    [Range(0f, 1f)] public float volume = 1f;
    public AudioCategorys category;
}
