using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    bool isTriggered = false;

    public bool IsTriggered { get => isTriggered; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        isTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }
}
