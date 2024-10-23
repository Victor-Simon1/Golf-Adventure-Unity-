using Services;
using UnityEngine;
using UnityEngine.UI;
public class OptionsUI : MonoBehaviour
{
    [Header("Gameobjects")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider effectsSlider;
    [SerializeField] Toggle vibrationToggle;
    [Header("Manager")]
    private SettingsManager mSettings;
    [Header("Variable")]
    bool startDone = false;

#region UNITY_FUNCTION
    private void Start()
    {
        startDone = true;
        SetOption();
       
    }
    private void OnEnable()
    {
        SetOption();
    }
#endregion

#region PRIVATE_FUNCTION

    //Set the gameobject to the correct value
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
#endregion
}
