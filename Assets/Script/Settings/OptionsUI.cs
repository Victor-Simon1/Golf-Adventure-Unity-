using Services;
using UnityEngine;
using UnityEngine.UI;
public class OptionsUI : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider effectsSlider;
    [SerializeField] Toggle vibrationToggle;
    private SettingsManager mSettings;
    bool startDone = false;
    private void Start()
    {
        startDone = true;
        SetOption();
       
    }
    private void OnEnable()
    {
        SetOption();
       /* mSettings = ServiceLocator.Get<SettingsManager>();
        if (mSettings == null)
            Debug.LogError("Settings Manager was not found ! ");
        musicSlider.value = mSettings.musicVolume;
        musicSlider.onValueChanged.AddListener(
                       delegate
                       {
                           mSettings.ModifyMusic(musicSlider.value);
                           mSettings.SaveSound();
                       });

        effectsSlider.value = mSettings.sfxVolume;
        effectsSlider.onValueChanged.AddListener(
                      delegate
                      {
                          mSettings.ModifyEffectsSounds(effectsSlider.value);
                          mSettings.SaveSound();
                      });

        vibrationToggle.isOn = mSettings.vibrateOn;
        vibrationToggle.onValueChanged.AddListener(
                    delegate
                    {
                        mSettings.ModifyVibrations(vibrationToggle.isOn);
                        mSettings.SaveSound();
                    });*/
    }

    void SetOption()
    {
        if (!startDone)
            return;
        mSettings = ServiceLocator.Get<SettingsManager>();
        mSettings.ApplySound();
        if (mSettings == null)
            Debug.LogError("Settings Manager was not found ! ");
        musicSlider.value = mSettings.musicVolume;
        musicSlider.onValueChanged.AddListener(
                       delegate
                       {
                           mSettings.ModifyMusic(musicSlider.value);
                           mSettings.SaveSound();
                       });

        effectsSlider.value = mSettings.sfxVolume;
        effectsSlider.onValueChanged.AddListener(
                      delegate
                      {
                          mSettings.ModifyEffectsSounds(effectsSlider.value);
                          mSettings.SaveSound();
                      });

        vibrationToggle.isOn = mSettings.vibrateOn;
        vibrationToggle.onValueChanged.AddListener(
                    delegate
                    {
                        mSettings.ModifyVibrations(vibrationToggle.isOn);
                        mSettings.SaveSound();
                    });
    }

}
