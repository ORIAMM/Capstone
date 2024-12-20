using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SoundCEO : MonoBehaviour
{
    public static SoundCEO instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup bgmGroup;

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
        if (Clip == null || Clip.clip == null)
        {
            Debug.LogWarning("MANAA SO NYA WO");
            return;
        }
        AudioSource audioSource = GetAvailableAudioSource();
        audioSource.clip = Clip.clip;
        audioSource.volume = Clip.volume;
        audioSource.loop = Clip.loop;
        switch (Clip.category)
        {
            case AudioCategorys.SFX:
                audioSource.outputAudioMixerGroup = sfxGroup;
                break;
            case AudioCategorys.BGM:
                audioSource.outputAudioMixerGroup = bgmGroup;
                break;
            default:
                audioSource.outputAudioMixerGroup = masterGroup;
                break;
        }
        //audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(Clip.category.ToString())[0];
        audioSource.Play();
        
    }
    public void StopAllSound()
    {
        foreach(var audioSource in audioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }
    public void ResumeAllSound()
    {
        foreach (var audioSource in audioSources)
        {
            if (audioSource != null && audioSource.time > 0 && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    public void PauseClip(SoundCLIP clip)
    {
        foreach (var audioSource in audioSources)
        {
            if (audioSource.isPlaying && audioSource.clip == clip)
            {
                audioSource.Pause();
            }
        }
    }

    public void ResumeClip(SoundCLIP clip)
    {
        foreach (var audioSource in audioSources)
        {
            if (!audioSource.isPlaying && audioSource.clip == clip)
            {
                audioSource.Play();
            }
        }
    }

    public float GetVolume(string parameterName)
    {
        if (audioMixer.GetFloat(parameterName, out float value))
        {
            return Mathf.Pow(10f, value / 20f);
        }
        return 1f;
    }

    public void SetVolume(AudioCategorys category, float volume)
    {
        if(volumeParameters.TryGetValue(category, out string ParameterName))
        {
            if (volume <= 0.01f)
            {
                audioMixer.SetFloat(ParameterName, -80f);
            }
            else
            {
                audioMixer.SetFloat(ParameterName, Mathf.Log10(volume) * 20);
            }
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
