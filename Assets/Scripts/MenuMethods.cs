using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuMethods : MonoBehaviour
{
    float delayDuration = 1f;
    public float DelayDuration { get => delayDuration; set => delayDuration = value; }

    EScenes currentScene = EScenes.MainMenuScene;
    public EScenes CurrentScene { get => currentScene; set { currentScene = value; } }

    EScenes previousScene = EScenes.Empty;

    public static MenuMethods Instance { get; private set; }

    private void Awake()
    {
        //singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Load()
    {
        StartCoroutine(Delay(Continue, PlayerReturnsToLastSave));
    }

    public void New()
    {
        StartCoroutine(Delay(StartTime,RestartGame));
        currentScene = EScenes.MainMenuScene; //Reset
        previousScene = EScenes.Empty; //Reset
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Settings()
    {
        StartCoroutine(Delay(() => SwitchScene(EScenes.SettingsScene)));
    }

    public void Credits()
    {
        StartCoroutine(Delay(() => SwitchScene(EScenes.CreditsScene)));
    }

    public void Manual()
    {
        StartCoroutine(Delay(() => SwitchScene(EScenes.ManualScene)));
    }

    public void StartPause()
    {
        StartCoroutine(Delay(ToggleTime, () => SwitchScene(EScenes.PauseScene)));
    }

    public void Continue()
    {
        StartCoroutine(Delay(StartTime, CloseCurrentScene));
    }

    public void QuickContinue()
    {
        StartTime();
        CloseCurrentScene();
    }

    public void Main()
    {
        StartCoroutine(Delay(() => SwitchScene(EScenes.MainMenuSceneAdditive)));
    }

    public void Return()
    {
        if (previousScene != EScenes.Empty)
        {
            StartCoroutine(Delay(() => SwitchScene(previousScene)));
        }
        else
        {
            StartTime();
            StartCoroutine(Delay(CloseCurrentScene));
        }
    }

    //public void Understand()
    //{
    //    StartCoroutine(Delay(() => SwitchScene(previousScene), StartTime));
    //}

    IEnumerator Delay(System.Action method, System.Action additionalMethod=null, System.Action anotherAdditionalMethod = null)
    {
        float timer = 0f;
        float seconds = delayDuration;

        while(timer < seconds)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        method();

        if (additionalMethod != null)
            additionalMethod();

        if (anotherAdditionalMethod != null)
            anotherAdditionalMethod();
    }

    void ToggleTime()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void StartTime()
    {
        if (Time.timeScale == 1f) return;

        Time.timeScale = 1f;
    }

    void StopTime()
    {
        if (Time.timeScale == 0f) return;

        Time.timeScale = 0f;
    }

    void PlayerReturnsToLastSave()
    {
        FindObjectOfType<PlayerController>().ResetAndReturnToLastSave();
    }

    void RestartGame()
    {
        SceneManager.LoadSceneAsync(EScenes.GameWorldScene.ToString(), LoadSceneMode.Single);
        currentScene = EScenes.Empty;
    }

    void SwitchScene(EScenes newScene)
    {
        if (currentScene != EScenes.Empty) //checks if there is a scene
        {
            previousScene = currentScene;
            CloseCurrentScene();
        }

        StartCoroutine(WaitForScene(newScene));
    }

    IEnumerator WaitForScene(EScenes newScene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newScene.ToString(), LoadSceneMode.Additive);
        currentScene = newScene;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        MakeSureCurrentSceneIsActiveScene();
    }

    void MakeSureCurrentSceneIsActiveScene()
    {
        Scene newSceneToLoad = SceneManager.GetSceneByName(currentScene.ToString());
        SceneManager.SetActiveScene(newSceneToLoad);
    }

    void CloseCurrentScene()
    {
        if (currentScene == EScenes.Empty) return;

        SceneManager.UnloadSceneAsync(currentScene.ToString());
        currentScene = EScenes.Empty;
    }

}
