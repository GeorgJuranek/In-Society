using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EndBossLogic : MonoBehaviour
{
    EnemyPlayerDetection[] childrenStatuses;

    int activatedTrapsCounter;

    [SerializeField]
    GameObject lightSwitch;

    AudioSource audioSource;

    public Action OnUpgradeGranted = delegate { };

    void Awake()
    {
        childrenStatuses = GetComponentsInChildren<EnemyPlayerDetection>();

        foreach (EnemyPlayerDetection childrenStatus in childrenStatuses)
        {
            childrenStatus.OnTrapActivated += OnTrapActivate;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void OnTrapActivate()
    {
        activatedTrapsCounter++;

        //Debug.Log($"Boss-Counter: {activatedTrapsCounter}");

        if(activatedTrapsCounter >= childrenStatuses.Length) 
        {
            //Debug.Log("Whole Bossfight was successful!");

            GrantUpgrade();
        }
    }

    void GrantUpgrade()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();

        player.DashGrowth *= 2f;

        if (lightSwitch != null)
        {
            lightSwitch.GetComponent<Activateable>().Disable();
            Destroy(lightSwitch);
        }

        OnUpgradeGranted?.Invoke();

        audioSource.Play();
    }
}
