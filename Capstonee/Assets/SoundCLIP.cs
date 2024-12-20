using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioCategorys
{
    SFX,
    BGM,
    Master
}

[CreateAssetMenu(fileName = "NewAudioClip", menuName = "Audio/AudioClips")]
public class SoundCLIP : ScriptableObject
{
    public AudioClip clip;
    public bool loop;
    [Range(0f, 1f)] public float volume = 1f;
    public AudioCategorys category;
}
