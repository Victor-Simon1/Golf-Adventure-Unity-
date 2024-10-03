using Services;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;
public class SettingsManager : MonoRegistrable
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private bool vibrateOn;

    [SerializeField] MusicManager musicManager;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        ServiceLocator.Register<SettingsManager>(this, false);
        audioMixer.SetFloat("volumeSFX", Mathf.Log10(0.5f) * 20);
        audioMixer.SetFloat("volumeMusic", Mathf.Log10(0.5f) * 20);
    }

    public void ModifyEffectsSounds(float value)
    {
        audioMixer.SetFloat("volumeSFX",Mathf.Log10(value)*20);
    }
    public void ModifyMusic(float value)
    {
        audioMixer.SetFloat("volumeMusic", Mathf.Log10(value) * 20);
    }
    public void ModifyVibrations(bool value)
    {
        vibrateOn = value;

    }
}
