using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeCon : MonoBehaviour
{
    [Header("Settings")]
    public Slider BGMSlider;
    public Slider SFXSlider;
    public Slider MasterSlider;


    public void Awake()
    {

        if (MasterSlider) MasterSlider.value = PlayerPrefs.GetFloat("Master", 1f);
        if (SFXSlider) SFXSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
        if (BGMSlider) BGMSlider.value = PlayerPrefs.GetFloat("BGM", 1f);

        MasterSlider.onValueChanged.AddListener(value => SoundCEO.instance.SetVolume(AudioCategorys.Master, value));
        SFXSlider.onValueChanged.AddListener(value => SoundCEO.instance.SetVolume(AudioCategorys.SFX, value));
        BGMSlider.onValueChanged.AddListener(value => SoundCEO.instance.SetVolume(AudioCategorys.BGM, value));
    }
}
