using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeChanger : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        slider.value = AudioListener.volume;

        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        AudioListener.volume = value;
    }
}
