using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class MouseSensitivityChanger : MonoBehaviour
{
    [SerializeField]
    float maxMouseSensitivityX = 300f, maxMouseSensitivityY = 2f;

    Slider slider;
    CinemachineFreeLook cam;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        cam = FindFirstObjectByType<CinemachineFreeLook>();

        if (cam != null)
        {
            slider.value = cam.m_XAxis.m_MaxSpeed/ maxMouseSensitivityX;
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        else
        {
            slider.enabled = false;
        }
    }

    public void OnSliderValueChanged(float value)
    {
        cam.m_XAxis.m_MaxSpeed = value * maxMouseSensitivityX;
        cam.m_YAxis.m_MaxSpeed = value * maxMouseSensitivityY;
    }
}
