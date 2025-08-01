using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;
using System;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] int numberNecessaryToOpen = 1;

    [SerializeField] float distanceToMoveUp = 5f;

    [SerializeField] float moveUpDelta = 0.01f;

    [SerializeField] GameObject minimapMark;

    [SerializeField] GameObject textMeshPro;

    [SerializeField] GameObject doorWall;

    AudioSource doorAudioSource;

    [SerializeField]
    Camera doorCam;

    public static Action<bool> OnDoorSceneIsHappening;

    bool isOpen;

    private void Awake()
    {
        ChangeText(0); //init
        doorAudioSource = GetComponent<AudioSource>();

        //doorCam = GetComponentInChildren<Camera>();
    }

    private void OnEnable()
    {
        PlayerController.OnCountChange += ChangeText;
        PlayerController.OnCountChange += OpenUp;
    }

    private void OnDisable()
    {
        PlayerController.OnCountChange -= ChangeText;
        PlayerController.OnCountChange -= OpenUp;
    }

    public void ChangeText(int newCount)
    {
        if (numberNecessaryToOpen - newCount < 0) return;

        textMeshPro.GetComponent<TextMeshPro>().text = (numberNecessaryToOpen - newCount).ToString();
    }

    public void OpenUp(int newCount)
    {
        if (newCount > numberNecessaryToOpen) return;

        if (newCount==numberNecessaryToOpen && !isOpen)
            StartCoroutine("MoveUp");
    }

    [SerializeField] float moveSpeed = 1f; // Units per second

    IEnumerator MoveUp()
    {
        OnDoorSceneIsHappening?.Invoke(true);

        if (doorCam != null)
        {
            doorCam.gameObject.GetComponent<CinemachineVirtualCamera>().Priority = 10;
        }

        doorAudioSource.Play();

        Vector3 startPos = doorWall.transform.position;
        Vector3 endPos = startPos + Vector3.up * distanceToMoveUp;

        while (Vector3.Distance(doorWall.transform.position, endPos) > 0.01f)
        {
            doorWall.transform.position = Vector3.MoveTowards(doorWall.transform.position, endPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isOpen = true;

        minimapMark.GetComponent<MeshRenderer>().enabled = false;

        if (doorCam != null)
        {
            doorCam.gameObject.GetComponent<CinemachineVirtualCamera>().Priority = -10;
        }

        OnDoorSceneIsHappening?.Invoke(false);
    }

}
