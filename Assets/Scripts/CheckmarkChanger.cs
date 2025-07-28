using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System;
using UnityEngine.EventSystems;

public class CheckmarkChanger : MonoBehaviour
{
    //[SerializeField]
    //float maxMouseSensitivityX = 300f, maxMouseSensitivityY = 2f;

    Toggle toggle;
    CinemachineFreeLook cam;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        cam = FindFirstObjectByType<CinemachineFreeLook>();

        if (cam != null)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        //else
        //{

        if (cam != null)
        {
            toggle.isOn = cam.m_XAxis.m_InvertInput;
        }
        else
        {
            toggle.isOn = false;
            toggle.interactable = false;
            toggle.enabled = false;
        }
        //}
    }

    public void OnToggleValueChanged(bool isOn)
    {
        if (cam == null) return;

        cam.m_XAxis.m_InvertInput = isOn; // !cam.m_XAxis.m_InvertInput;
        cam.m_YAxis.m_InvertInput = !isOn;
        //EventSystem.current.SetSelectedGameObject(this.gameObject);// toggle.navigation.selectOnDown.gameObject);
        EventSystem.current.SetSelectedGameObject(toggle.navigation.selectOnDown.gameObject);
    }
}
