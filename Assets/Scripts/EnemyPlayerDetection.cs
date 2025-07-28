using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class EnemyPlayerDetection : MonoBehaviour
{
    [SerializeField]
    Material wallStoneMaterial;

    [SerializeField] ParticleSystem particles;

    [SerializeField] Light glowLight;

    [SerializeField] GameObject searcher;

    bool hasTrapSucceeded;
    //public bool HasTrapSucceeded => hasTrapSucceeded;

    bool isTrapSet;

    //
    public Action OnTrapActivated = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == searcher)
        {
            isTrapSet = true;
        }
    }

        private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Dummy"))
        {
            if (isTrapSet && !hasTrapSucceeded)
            {
                StartCoroutine(TrapExecution(other));
                hasTrapSucceeded = true;
            }
        }

        if (other.CompareTag("ExDummy"))//is permanent
        {
            Destroy(searcher);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == searcher)
        {
            isTrapSet = false;
        }

        if (other.CompareTag("Player"))
        {
            hasTrapSucceeded = false;
        }
    }

    IEnumerator TrapExecution(Collider other)
    {
        if (other.CompareTag("Dummy"))
        {
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            other.transform.parent = transform;
            other.tag = "ExDummy";

            OnTrapActivated?.Invoke();
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            other.GetComponentInParent<PlayerController>().IsStopped = true;

        }

        particles.Play();

        while (particles.time < particles.main.duration*3/4 )
        {
            yield return new WaitForSeconds(0.1f);
        }

        other.GetComponent<Renderer>().material = wallStoneMaterial;
        glowLight.enabled = true;

        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("DeathAdditionalScene", LoadSceneMode.Additive);
            FindAnyObjectByType<MenuMethods>().CurrentScene = EScenes.DeathAdditionalScene; // "DeathAdditionalScene";
        }
    }
}
