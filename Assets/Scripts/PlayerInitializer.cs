using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField]
    GameObject cam;

    CinemachineFreeLook freeLookCam;
    CinemachineInputProvider inputProviderOfCam;

    Transform rememberOriginalLookAt;

    private void Awake()
    {
        freeLookCam = cam.GetComponent<CinemachineFreeLook>();
        inputProviderOfCam = cam.GetComponent<CinemachineInputProvider>();

        rememberOriginalLookAt = freeLookCam.LookAt;

        inputProviderOfCam.enabled = false;

        freeLookCam.LookAt = this.transform;
    }

    private void Start()
    {
        CallManual();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SettingsLightSingleton.Instance.GetComponent<LockOnTarget>().Target = other.transform; //the Singleton gets a new target

            other.GetComponent<Rigidbody>().velocity = Vector3.up*4f; //purely cinematic so magic number

            freeLookCam.LookAt = rememberOriginalLookAt;
            inputProviderOfCam.enabled = true;

            //CallManual();

            if (other.GetComponent<PlayerController>().IsStopped)
            {
                other.GetComponent<PlayerController>().IsStopped = false;
            }

            Destroy(this);
        }
    }

    void CallManual()
    {
        Time.timeScale = 0f;

        MenuMethods sceneChange = FindObjectOfType<MenuMethods>();

        sceneChange.CurrentScene = EScenes.ManualScene;
        SceneManager.LoadScene(EScenes.ManualScene.ToString(), LoadSceneMode.Additive);
    }
}
