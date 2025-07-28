using UnityEngine;
using UnityEngine.UI;

public class BrightnessChanger : MonoBehaviour
{
    Slider slider;

    Light settingsLight;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = 0f;

        slider.onValueChanged.AddListener(OnSliderValueChanged);

        settingsLight = SettingsLightSingleton.Instance.GetComponent<Light>();

        if (settingsLight != null)
            slider.value = settingsLight.intensity;
    }

    public void OnSliderValueChanged(float value)
    {
        settingsLight.intensity = value;
    }
}
