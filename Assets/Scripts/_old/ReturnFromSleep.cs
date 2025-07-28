using UnityEngine;

public class ReturnFromSleep : MonoBehaviour
{
    bool isActivated;

    public bool IsActivated { get => isActivated; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Activator"))
        {
            isActivated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Activator"))
        {
            isActivated = false;
        }
    }
}
