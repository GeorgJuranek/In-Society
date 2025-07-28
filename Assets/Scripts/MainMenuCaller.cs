using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuCaller : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadSceneAsync(EScenes.MainMenuScene.ToString(), LoadSceneMode.Additive);
    }
}
