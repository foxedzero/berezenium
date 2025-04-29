using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject ResetPanel;

    [SerializeField] private GameObject MapGenerationPanel;

    [SerializeField] private FirstScene FirstScene;

    [SerializeField] private Text Loading;
    [SerializeField] private AudioSource Music;
    private bool Started = false;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerWinrate"))
        {
            PlayerPrefs.SetInt("PlayerWinrate", 0);
            PlayerPrefs.Save();
        }

        CursorManager.SetNeedMouse(new NeedCursorOrder(), false);

        StartCoroutine(PlayMusic());

        UserContent._Instance.UpdateMods();
    }

    public void StartGame()
    {
        SaveData data = FirstScene.GetData();
        if (data == null)
        {
            MapGenerationPanel.SetActive(true);
            MainMenuPanel.SetActive(false);
            return;
        }

        if(!Started)
        {
            SaveManager._Instance.SetData(data);
            Started = true;
            StartCoroutine(LoadScene());
        }
    }

    public void AnswerSeed(bool state)
    {
        MapGenerationPanel.SetActive(false);
        MainMenuPanel.SetActive(true);

        if (state)
        {
            StartGame();
        }
    }

    public void NewGame()
    {
        ResetPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }
    public void NewGame(bool state)
    {
        ResetPanel.SetActive(false);
        MainMenuPanel.SetActive(true);

        if (state)
        {
            PlayerPrefs.SetInt("Exposition", 0);
            PlayerPrefs.Save();

            File.Delete(Path.Combine(Application.persistentDataPath, "LocalSave.json"));
            SceneManager.LoadScene(0);
        }
    }

    public void ToBearRedactor()
    {
        SceneManager.LoadScene(5);
    }

    public void OpenModsDirectory()
    {
        string path = Path.Combine(Application.dataPath, UserContent._DirectoryName);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        Application.OpenURL(path);
    }

    public void UpdateMods()
    {
        UserContent._Instance.UpdateMods();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    private IEnumerator LoadScene()
    {
        MainMenuPanel.SetActive(false);

        AsyncOperation operation = null;
        if (PlayerPrefs.GetInt("Exposition") == 0)
        {
            operation = SceneManager.LoadSceneAsync(2);
        }
        else
        {
            operation = SceneManager.LoadSceneAsync(1);
        }

        operation.allowSceneActivation = false;

        Loading.text = "Инициализация уровня...";

        float timer = 1;

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            Music.volume = timer;
            yield return waitForEndOfFrame;
        }

        operation.allowSceneActivation = true;
    }

    private IEnumerator PlayMusic()
    {
        yield return new WaitForSecondsRealtime(3);

        Music.Play();

        while (Music.volume < 1)
        {
            Music.volume += Time.unscaledDeltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }
    }
}
