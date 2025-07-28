using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class DashDisplay : MonoBehaviour
{
    [SerializeField] GameObject dashVisual;
    Image dashDisplay;

    float currentAmount = 0;

    //public Action OnUpgradeGranted = delegate { };
    //EndBossLogic[] bosses = new EndBossLogic[2];

    bool firstUpgradeWasAlreadyGranted = false;

    [SerializeField]
    Color[] UpgradeColors = new Color[2];

    void Awake()
    {
        dashDisplay = dashVisual.GetComponent<Image>();
        dashDisplay.fillAmount = currentAmount;

        EndBossLogic[] bosses = FindObjectsOfType<EndBossLogic>();

        foreach(EndBossLogic boss in bosses)
        {
            boss.OnUpgradeGranted += ChangeColorBecauseUpgraded;
        }
    }

    void OnEnable()
    {
        PlayerController.OnDashChange += HandleDashLoad;
    }

    void OnDisable()
    {
        PlayerController.OnDashChange -= HandleDashLoad;
    }

    void HandleDashLoad(float newAmount)
    {
        dashDisplay.fillAmount = newAmount;
    }

    void ChangeColorBecauseUpgraded()
    {
        if (!firstUpgradeWasAlreadyGranted)
        {
            dashDisplay.color = UpgradeColors[0];
            firstUpgradeWasAlreadyGranted = true;
        }
        else
        {
            dashDisplay.color = UpgradeColors[1];
        }
    }
}
