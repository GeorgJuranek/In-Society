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

    IEnumerator MoveUp()
    {
        OnDoorSceneIsHappening?.Invoke(true);


        //test
        if (doorCam != null)
        {
            doorCam.gameObject.GetComponent<CinemachineVirtualCamera>().Priority = 10;
            //Time.timeScale = 0.1f;
        }
        //test end

        doorAudioSource.Play();

        float startPosition = doorWall.transform.position.y;
        float endPosition = doorWall.transform.position.y + distanceToMoveUp;

        while(startPosition < endPosition)
        {
            doorWall.transform.position = Vector3.MoveTowards(doorWall.transform.position, new Vector3(doorWall.transform.position.x,endPosition, doorWall.transform.position.z), moveUpDelta);
            startPosition = doorWall.transform.position.y;

            yield return null;
        }

        isOpen = true;

        minimapMark.GetComponent<MeshRenderer>().enabled = false;

        //test
        if (doorCam != null)
        {
            doorCam.gameObject.GetComponent<CinemachineVirtualCamera>().Priority = -10;
            //Time.timeScale = 1f;
        }
        //Destroy(doorCam.gameObject);


        OnDoorSceneIsHappening?.Invoke(false);
    }
}
