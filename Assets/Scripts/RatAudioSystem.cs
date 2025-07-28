using UnityEngine;

public class RatAudioSystem : MonoBehaviour
{
    AudioSource ratAudio;

    [SerializeField] AudioClip activationSound;

    [SerializeField] AudioClip[] ratChorus;
    [SerializeField] LayerMask ground;

    bool soundOverPopulation = false;

    int addedSinceOverPopultion;

    private void Awake()
    {
        ratAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PlayerController.OnOverPopulation += OverPopulationReached;
    }

    private void OnDisable()
    {
        PlayerController.OnOverPopulation -= OverPopulationReached;
    }

    //private void OnEnable()
    //{
    //    CallInitSound();
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (ground == (ground | (1 << other.gameObject.layer)))
        {
            if (soundOverPopulation)
            {
                if (Random.Range(0,addedSinceOverPopultion) == 0)
                {
                    int randomIndex = Random.Range(0, ratChorus.Length - 1);
                    ratAudio.clip = ratChorus[randomIndex];
                    ratAudio.Play();
                }
            }
            else
            {
                int randomIndex = Random.Range(0, ratChorus.Length - 1);
                ratAudio.clip = ratChorus[randomIndex];
                ratAudio.Play();
            }
        }
    }

    //void CallInitSound()
    //{
    //    if (GetComponentInParent<RatBrain>().IsTrapped) return;
    //    
    //    float playDelay = Random.Range(0f, 3f);
    //    Invoke("PlayActivationSound", playDelay);    
    //}
    //
    //void PlayActivationSound()
    //{
    //    ratAudio.clip = activationSound;
    //    ratAudio.Play();
    //}

    void OverPopulationReached()
    {
        addedSinceOverPopultion++;

        if (!soundOverPopulation)
        soundOverPopulation = true;

        //ratAudio.pitch += 0.01f;
    }
}
