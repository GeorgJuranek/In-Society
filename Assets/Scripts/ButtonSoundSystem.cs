using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class ButtonSoundSystem : MonoBehaviour
{
    InputAction move;
    InputAction confirm;

    RatKingInputs playerInput;

    AudioSource audioSource;

    [SerializeField]
    private AudioClip[] audioClips;

    private Button[] buttons;

    GameObject lastSelected;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();

        audioSource = GetComponent<AudioSource>();

        playerInput = new RatKingInputs();

        move = playerInput.RatKingActionmap.Movement;
        confirm = playerInput.RatKingActionmap.Confirm; //Jump

        //currentSelected = EventSystem.current.currentSelectedGameObject;
    }

    private void OnEnable()
    {
        move.Enable();
        confirm.Enable();

        move.canceled += PlayButtonSwitchSound;
        confirm.canceled += ConfirmWithSound;
    }

    private void OnDisable()
    {
        move.Disable();
        confirm.Disable();

        move.canceled -= PlayButtonSwitchSound;
        confirm.canceled -= ConfirmWithSound;
    }

    private void PlayButtonSwitchSound(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject == lastSelected) return;

        int index = -1;

        foreach (Button button in buttons)
        {
            index++;

            if (EventSystem.current.currentSelectedGameObject == button.gameObject)
            {
                audioSource.clip = audioClips[index];
                audioSource.Play();

                lastSelected = button.gameObject;

                if (index+1 > buttons.Length-1)
                {
                    index = -1;
                }
            }

        }
    }

    private void ConfirmWithSound(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject != null &&
            EventSystem.current.currentSelectedGameObject.GetComponent<Slider>() != null) return;

        EventSystem.current.enabled = false;
        OnDisable();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, audioClips.Length);

            float lengthToWait = audioClips[randomIndex].length + 0.1f;

            FindAnyObjectByType<MenuMethods>().DelayDuration = lengthToWait;

            //if (audioSource != null)
                audioSource.PlayOneShot(audioClips[randomIndex]);
        }

        //Debug.Log("ConfirmWithSound was called");
    
    }
}
