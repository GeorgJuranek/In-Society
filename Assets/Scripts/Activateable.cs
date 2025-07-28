using UnityEngine;
using UnityEngine.Events;

public class Activateable : MonoBehaviour
{
    void OnEnable()
    {
        Disable();
    }

    [SerializeField]
    private UnityEvent objectEnabled;    
    
    [SerializeField]
    private UnityEvent objectDisabled;

    public void Enable()
    {
        objectEnabled.Invoke();
    }

    public void Disable()
    {
        objectDisabled.Invoke();
    }
}

