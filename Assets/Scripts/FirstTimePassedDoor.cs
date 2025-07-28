using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTimePassedDoor : MonoBehaviour
{
    bool hasPlayedAlready = false;

    private void OnTriggerExit(Collider other)
    {
        if (!hasPlayedAlready && other.CompareTag("Player"))
        {
            hasPlayedAlready = true;

            GetComponent<AudioSource>().Play();

        }
    }
}
