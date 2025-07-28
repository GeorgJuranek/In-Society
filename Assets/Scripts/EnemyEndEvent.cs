using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEndEvent : MonoBehaviour
{
    private void OnEnable()
    {
        WaitingForTheEnd.OnGameEnd += Deactivate;
    }

    private void OnDisable()
    {
        WaitingForTheEnd.OnGameEnd -= Deactivate;
    }


    void Deactivate()
    {
        Activateable activateable = GetComponent<Activateable>();
        activateable.Disable();
        Destroy(activateable);

        StartCoroutine(LastBreath());
    }


    IEnumerator LastBreath()
    {
        ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
        particles.Play();


        while (particles.isEmitting)
        {

            yield return new WaitForSeconds(0.1f);

        }

        while (particles.particleCount > 0)
        {

            yield return new WaitForSeconds(0.1f);

        }

        Destroy(this.gameObject);
    }
}
