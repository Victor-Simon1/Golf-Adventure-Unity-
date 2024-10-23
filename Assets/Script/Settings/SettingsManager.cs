using Services;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoRegistrable
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [Header("Variable")]
    [SerializeField] public bool vibrateOn;
    [SerializeField] public float sfxVolume = 0.5f,musicVolume = 0.5f;

#region UNITY_FUNCTION
    // Start is called before the first frame update
    void Start()
    {
      
        //Set Float of AudioMixer dont work in awake
        DontDestroyOnLoad(this);
        if (ServiceLocator.IsRegistered<SettingsManager>())
            Destroy(ServiceLocator.Get<SettingsManager>().gameObject);
        ServiceLocator.Register<SettingsManager>(this, false);
        //Set default value if key dont exist, else take current value
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

#endregion

#region PUBLIC_FUNCTION
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
#endregion
}
