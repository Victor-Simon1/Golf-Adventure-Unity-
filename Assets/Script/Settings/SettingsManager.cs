using Services;
using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;
public class SettingsManager : MonoRegistrable
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] public bool vibrateOn;
    [SerializeField] public float sfxVolume = 0.5f,musicVolume = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        ServiceLocator.Register<SettingsManager>(this, false);
        if(PlayerPrefs.HasKey("Vibration"))
        {
            Debug.Log("Valeurs déja enregistré");
            ApplySound();
        }
        else
        {
            Debug.Log("Valeurs non enregistré");
            audioMixer.SetFloat("volumeSFX", Mathf.Log10(0.5f) * 20);
            audioMixer.SetFloat("volumeMusic", Mathf.Log10(0.5f) * 20);
            SaveSound();
        }
       
    }

    public void ModifyEffectsSounds(float value)
    {
        audioMixer.SetFloat("volumeSFX",Mathf.Log10(value)*20);
        sfxVolume = value;
    }
    public void ModifyMusic(float value)
    {
        audioMixer.SetFloat("volumeMusic", Mathf.Log10(value) * 20);
        musicVolume = value;
    }
    public void ModifyVibrations(bool value)
    {
        vibrateOn = value;
    }
    public void SaveSound()
    {
        PlayerPrefs.SetFloat("EffectSFXVolume", sfxVolume);

        PlayerPrefs.SetFloat("MusicSFXVolume", musicVolume);

        PlayerPrefs.SetInt("Vibration", Convert.ToInt32(vibrateOn));
    }

    public void ApplySound()
    {
        float valueSFX =  PlayerPrefs.GetFloat("EffectSFXVolume");
        sfxVolume = valueSFX;
        ModifyEffectsSounds(valueSFX);

        float valueMusic = PlayerPrefs.GetFloat("MusicSFXVolume");
        musicVolume = valueMusic;
        ModifyMusic(valueMusic);

        bool vibration = Convert.ToBoolean(PlayerPrefs.GetInt("Vibration"));
        ModifyVibrations(vibration);
    }

}
