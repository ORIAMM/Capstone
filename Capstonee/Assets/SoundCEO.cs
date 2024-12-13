using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SoundCEO : MonoBehaviour
{
    public static SoundCEO instance;
    public AudioMixer audioMixer;
    public Slider Sfxslider;
    public Slider BGMslider;
    public Slider Masterslider;

    [Header("Audio Mixer")]

    private List<AudioSource> audioSources = new List<AudioSource>();
    private Dictionary<AudioCategorys, string> volumeParameters = new Dictionary<AudioCategorys, string>
    {
        {AudioCategorys.Master, "Master" },
        {AudioCategorys.SFX, "SFX" },
        {AudioCategorys.BGM, "BGM" },
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
        LoadVolumeSettings();
    }
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying) return source;

        }
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        audioSources.Add(newAudioSource);
        return newAudioSource;
    }

    public void PlaySound(SoundCLIP Clip)
    {
        if (Clip == null || Clip.clips == null)
        {
            Debug.LogWarning("MANAA SO NYA WO");
            return;
        }
        AudioSource audioSource = GetAvailableAudioSource();
        foreach(AudioClip clip in Clip.clips)
        {
            
        }
        audioSource.volume = Clip.volume;
        audioSource.loop = Clip.loop;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(Clip.category.ToString())[0];
        audioSource.Play();
        
    }
    public void StopAllSound()
    {
        foreach(var audioSource in audioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void SetVolume(AudioCategorys category, float volume)
    {
        if(volumeParameters.TryGetValue(category, out string ParameterName))
        {
            audioMixer.SetFloat(ParameterName, Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat(ParameterName, volume);
        }
    }

    private void LoadVolumeSettings()
    {
        foreach(var entry in volumeParameters)
        {
            float savedVolume = PlayerPrefs.GetFloat(entry.Value, 1f);
            audioMixer.SetFloat(entry.Value, Mathf.Log10(savedVolume) * 20);

        }
    }

}
