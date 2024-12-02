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

    private float masterVolume;

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
            MusicVolume(masterSlider.value);
            PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        }
        if (musicSlider != null)
        {
            MusicVolume(musicSlider.value);
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        }
        if (sfxSlider != null)
        {
            SFXVolume(sfxSlider.value);
            PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        }
    }
    public void LoadPref()
    {
        // Load Master Volume
        if (PlayerPrefs.HasKey("MasterVolume") && masterSlider != null)
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            masterSlider.value = masterVolume; // Set slider value
            MasterVolume(masterVolume);       // Apply volume
        }

        // Load Music Volume
        if (PlayerPrefs.HasKey("MusicVolume") && musicSlider != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicSlider.value = musicVolume; // Set slider value
            MusicVolume(musicVolume);        // Apply volume
        }

        // Load SFX Volume
        if (PlayerPrefs.HasKey("SFXVolume") && sfxSlider != null)
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            sfxSlider.value = sfxVolume; // Set slider value
            SFXVolume(sfxVolume);        // Apply volume
        }
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
        Sound s = Array.Find(sfx, sfx => sfx.name == name);
        if (s == null)
        {
            return;
        }
        s.source.Play();
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, music => music.name == name);
        if (s == null)
        {
            return;
        }
        s.source.volume = 0f;
        s.source.Play();
    }

    public void StopMusic(string name)
    {
        Sound s = Array.Find(music, music => music.name == name);
        if (s == null)
        {
            return;
        }
    }

    public void StopAllMusic()
    {
        foreach (Sound s in music)
        {
            s.source.Stop();
        }
    }
    public void StopAllSfx()
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
            s.source.volume = volume;
        }
    }
    public void SFXVolume(float volume)
    {
        foreach (Sound s in sfx)
        {
            s.source.volume = volume;
        }
    }
    public void MasterVolume(float volume)
    {
        masterVolume = volume;

        // Scale music volume
        foreach (Sound s in music)
        {
            s.source.volume = s.Volume * musicSlider.value * masterVolume;
        }

        // Scale SFX volume
        foreach (Sound s in sfx)
        {
            s.source.volume = s.Volume * sfxSlider.value * masterVolume;
        }
    }
    public void PauseSfx()
    {
        for (int i = 0; i < 6; i++)
        {
            sfx[i].source.Pause();
        }
    }
    public void UnPauseSfx()
    {
        for (int i = 0; i < 6; i++)
        {
            sfx[i].source.UnPause();
        }
    }

    public void PauseMusic(string name)
    {
        Sound s = Array.Find(music, music => music.name == name);
        if (s == null)
        {
            return;
        }
        s.source.Pause();
    }

    public void UnpauseMusic(string name)
    {
        Sound s = Array.Find(music, music => music.name == name);
        if (s == null)
        {
            return;
        }
        s.source.UnPause();
    }
}
