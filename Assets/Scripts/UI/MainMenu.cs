using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private Text Loading;
    [SerializeField] private AudioSource Music;
    private bool Started = false;

    private void Start()
    {
        StartCoroutine(PlayMusic());
    }

    public void StartGame()
    {
        if(!Started)
        {
            Started = true;
            StartCoroutine(LoadScene());
        }
    }

    public void NewGame()
    {
        UserInteract.AskConfirm("Удалить данные", "Локальные и облачные данные игры будут удалены, как и сам игрок.\nВам снова придётся создать игрока.\nВы точно хотите этого ?", NewGame);
    }
    public void NewGame(bool state)
    {
        if (state)
        {
            NtoServerInterface.DeletePlayer(FindObjectOfType<SaveManager>()._PlayerName, NewGame);
        }
    }
    public void NewGame(string info)
    {
        PlayerPrefs.SetInt("Exposition", 0);
        PlayerPrefs.Save();

        File.Delete(Path.Combine(Application.persistentDataPath, "LocalSave.json"));
        SceneManager.LoadScene(0);
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
