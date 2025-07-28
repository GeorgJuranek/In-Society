using System;
using System.Collections;
using UnityEngine;

public class WaitingForTheEnd : MonoBehaviour
{
    int maxCount;

    [SerializeField] Light directionalLight;
    [SerializeField] float maxLightIntensity;

    [SerializeField] GameObject gui;
    [SerializeField] GameObject endText;
    [SerializeField] GameObject player;

    [SerializeField] float maxHeight = 300f;
    [SerializeField] float upwartsSpeed = 0.005f;


    public static Action OnGameEnd; 

    private void Awake()
    {
        maxCount = FindObjectsOfType<RatBrain>().Length;
    }

    private void OnEnable()
    {
        PlayerController.OnCountChange += GameEnd;
    }

    private void OnDisable()
    {
        PlayerController.OnCountChange -= GameEnd;
    }

    void GameEnd(int currentRats)
    {
        if (currentRats < maxCount) return;

        OnGameEnd?.Invoke();

        gui.SetActive(false);
        endText.SetActive(true);

        StartCoroutine("EndTime");

        StartCoroutine("BallToTheSky");
    }

    IEnumerator EndTime()
    {
        while (directionalLight.intensity < maxLightIntensity-0.1f)
        {
            float endLightIntensity = directionalLight.intensity;
            directionalLight.intensity = Mathf.Lerp(endLightIntensity, maxLightIntensity, Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator BallToTheSky()
    {
        Vector3 targetPosition = new Vector3(player.transform.forward.x*300, maxHeight, player.transform.forward.x * 300);

        player.GetComponent<PlayerController>().HasGameEnded = true;
        player.GetComponent<PlayerController>().IsStopped = true; //hasGameEnded is important for this method

        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        playerRigidbody.useGravity = false;

        while (player.transform.position.y < 50f )
        {
            playerRigidbody.AddForce(Vector3.up * upwartsSpeed, ForceMode.Acceleration);
            yield return null;
        }

        playerRigidbody.isKinematic = true;

        while (player.transform.position.y < maxHeight)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, 3f * Time.deltaTime);
            yield return null;
        }

        playerRigidbody.constraints = RigidbodyConstraints.FreezePosition;
    }
}
