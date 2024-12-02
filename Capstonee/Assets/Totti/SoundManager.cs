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
    public Slider musicSlider;
    public Slider sfxSlider;
    public Sound[] music;
    public Sound[] sfx;
    public static SoundManager instance;
    public float fadeDuration;
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
    public void getSlider()
    {
    }
    private void Start()
    {
    }

    private void Update()
    {
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
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.LogWarning("Sound: " + name + " playing");
        s.source.Play();
    }


    public void ChangeMusic(string from, string to)
    {
        Debug.Log("changin" + from + "to" + to);
        Sound s = Array.Find(music, music => music.name == from);
        Sound ss = Array.Find(music, music => music.name == to);
        ss.source.volume = 0f;
        ss.source.Play();
        //ss.source.DOFade(ss.Volume, fadeDuration);
        //s.source.DOFade(0f, fadeDuration).OnComplete(() => s.source.Stop());
    }
    public void PlayMusic(string name)
    {
        Debug.Log("Playing" + name);
        Sound s = Array.Find(music, music => music.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = 0f;
        s.source.Play();
        //s.source.DOFade(s.Volume, fadeDuration);
    }

    public void StopMusic(string name)
    {
        Debug.Log("Stopping" + name);
        Sound s = Array.Find(music, music => music.name == name);
        if (s == null)
        {
            return;
        }
        //s.source.DOFade(0f, fadeDuration).OnComplete(() => s.source.Stop());
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
    public void StopAll()
    {
        foreach (Sound s in sfx)
        {
            s.source.Stop();
        }
        foreach (Sound s in music)
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
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Pause();
    }

    public void UnpauseMusic(string name)
    {
        Sound s = Array.Find(music, music => music.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.UnPause();
    }
    public void ChangeMusic()
    {

    }
}
