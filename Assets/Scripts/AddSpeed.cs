using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSpeed : MonoBehaviour
{
    bool isActive = true;

    [SerializeField]
    ParticleSystem speedParticles;

    [SerializeField]
    Light speedTileLight;

    float startIntensity;

    [SerializeField]
    float timeTillIsLoadedAgain = 5f;

    AudioSource audioSourceOfSpeed;

    private void OnDisable()
    {
        if (speedParticles.isStopped)
        StopCoroutine("LoadActivation");
    }

    private void Awake()
    {
        startIntensity = speedTileLight.intensity;
        audioSourceOfSpeed = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            if (other.GetComponent<PlayerController>())
            {
                audioSourceOfSpeed.Play();

                PlayerController kingsSpeed = other.GetComponent<PlayerController>();

                kingsSpeed.DashRelease(kingsSpeed.MaxDashSpeed);

                isActive = false;

                StartCoroutine("LoadActivation");
            }
        }
    }

    IEnumerator LoadActivation()
    {
        speedParticles.Stop();
        speedTileLight.intensity = startIntensity/4f;

        float start = 0f;

        while(start < timeTillIsLoadedAgain)
        {
            start ++;
            yield return new WaitForSeconds(1f);
        }

        isActive = true;

        speedTileLight.intensity = startIntensity;
        speedParticles.Play();
    }
}
