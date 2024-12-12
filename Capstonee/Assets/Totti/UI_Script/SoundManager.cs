using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public bool loop;

        [Range(0f, 1f)]
        public float Volume = 1.0f;

        [HideInInspector]
        public AudioSource source;
    }

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private float masterVolume = 1f;

    public Sound[] music;
    public Sound[] sfx;

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        InitializeSounds(music);
        InitializeSounds(sfx);
    }

    private void Start()
    {
        LoadPref();
    }

    public void SavePref()
    {
        if (masterSlider != null)
        {
            PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        }

        if (musicSlider != null)
        {
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        }

        if (sfxSlider != null)
        {
            PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        }

        PlayerPrefs.Save();
        ApplyVolume();
        Debug.Log("Preferences Saved!");
    }

    public void LoadPref()
    {
        if (masterSlider != null && PlayerPrefs.HasKey("MasterVolume"))
        {
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }

        if (musicSlider != null && PlayerPrefs.HasKey("MusicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        if (sfxSlider != null && PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }

        ApplyVolume();
    }

    private void InitializeSounds(Sound[] sounds)
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.Volume;
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfx, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
        }
    }

    public void StopMusic(string name)
    {
        Sound s = Array.Find(music, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop();
        }
    }

    public void StopSFX(string name)
    {
        Sound s = Array.Find(sfx, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop();
        }
    }

    public void StopAllMusic()
    {
        foreach (Sound s in music)
        {
            s.source.Stop();
        }
    }

    public void StopAllSFX()
    {
        foreach (Sound s in sfx)
        {
            s.source.Stop();
        }
    }

    public void MusicVolume(float volume)
    {
        foreach (Sound s in music)
        {
            s.source.volume = s.Volume * volume * (masterSlider != null ? masterSlider.value : 1f);
        }
    }

    public void SFXVolume(float volume)
    {
        foreach (Sound s in sfx)
        {
            s.source.volume = s.Volume * volume * (masterSlider != null ? masterSlider.value : 1f);
        }
    }

    public void MasterVolume(float volume)
    {
        masterVolume = volume;
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        float master = masterSlider != null ? masterSlider.value : 1f;
        float musicVol = musicSlider != null ? musicSlider.value : 1f;
        float sfxVol = sfxSlider != null ? sfxSlider.value : 1f;

        foreach (Sound s in music)
        {
            s.source.volume = s.Volume * musicVol * master;
        }

        foreach (Sound s in sfx)
        {
            s.source.volume = s.Volume * sfxVol * master;
        }

        //Debug.Log("Volume Applied!");
    }

    public bool IsSFXPlaying(string name)
    {
        Sound s = Array.Find(sfx, sound => sound.name == name);
        return s != null && s.source.isPlaying;
    }

    public void PauseMusic()
    {
        foreach (Sound s in music)
        {
            if (s.source.isPlaying)
            {
                s.source.Pause();
            }
        }
    }

    public void PauseSfx()
    {
        foreach (Sound s in sfx)
        {
            if (s.source.isPlaying)
            {
                s.source.Pause();
            }
        }
    }

    public void UnPauseMusic()
    {
        foreach (Sound s in music)
        {
            if (!s.source.isPlaying)
            {
                s.source.UnPause();
            }
        }
    }


    public void UnPauseSfx()
    {
        foreach (Sound s in sfx)
        {
            if (!s.source.isPlaying)
            {
                s.source.UnPause();
            }
        }
    }

}
