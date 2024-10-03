using Services;
using UnityEngine;
using UnityEngine.UI;
public class OptionsUI : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider effectsSlider;
    [SerializeField] Toggle vibrationToggle;
    private SettingsManager mSettings;
    private void OnEnable()
    {
        mSettings = ServiceLocator.Get<SettingsManager>();
        if (mSettings == null)
            Debug.LogError("Settings Manager was not found ! ");

        musicSlider.onValueChanged.AddListener(
                       delegate
                       {
                           mSettings.ModifyMusic(musicSlider.value);
                       });

        effectsSlider.onValueChanged.AddListener(
                      delegate
                      {
                          mSettings.ModifyEffectsSounds(effectsSlider.value);
                      });

        vibrationToggle.onValueChanged.AddListener(
                    delegate
                    {
                        mSettings.ModifyVibrations(vibrationToggle.isOn);
                    });
    }


}
