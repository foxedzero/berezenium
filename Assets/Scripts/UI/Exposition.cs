using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Exposition : MonoBehaviour
{
    [SerializeField] private Image CG;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private Text Text;

    [SerializeField] private Fraze[] Frazes;
    [SerializeField] private int CurrentFraze;
    private bool Loading = false;

    private Coroutine PrintCoroutine = null;

    private void Start()
    {
        PlayerPrefs.SetInt("Exposition", 1);
        PlayerPrefs.Save();
    }

    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.Escape))
        {
            Skip();
        }
    }
    public void Skip()
    {
        CurrentFraze = Frazes.Length;

        if (!Loading)
        {
            Loading = true;
            StopAllCoroutines();
            StartCoroutine(LoadScene());
        }
    }

    public void Next()
    {
        if (PrintCoroutine != null)
        {
            StopCoroutine(PrintCoroutine);
            PrintCoroutine = null;
            Text.text = Frazes[CurrentFraze].Text;
            return;
        }
        CurrentFraze++;

        if(CurrentFraze >= Frazes.Length)
        {
            if (!Loading)
            {
                Loading = true;
                StopAllCoroutines();
                StartCoroutine(LoadScene());
            }
            return;
        }

        if (Frazes[CurrentFraze].Change)
        {
            StartCoroutine(ChangeCGandBGM());
        }

        PrintCoroutine = StartCoroutine(PrintText());
    }

    private IEnumerator LoadScene()
    {
        Text.text = $"Инициализация уровня...";
        Loading = true;

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        yield return waitForEndOfFrame;
        yield return waitForEndOfFrame;

        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        operation.allowSceneActivation = false;

        float timer = 1;

        while (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            AudioSource.volume = timer;
            CG.color = Color.white * 0.85f * timer;
            yield return waitForEndOfFrame;
        }

        operation.allowSceneActivation = true;
    }

    private IEnumerator ChangeCGandBGM()
    {
        float value = 1;
        bool changeBgm = AudioSource.clip != Frazes[CurrentFraze].Audio;

        while(value > 0)
        {
            value -= Time.unscaledDeltaTime;

            if (changeBgm)
            {
                AudioSource.volume = value;
            }

            CG.color = Color.white * 0.85f * value;
            yield return new WaitForEndOfFrame();
        }

        CG.sprite = Frazes[CurrentFraze].CG;

        if (changeBgm)
        {
            AudioSource.clip = Frazes[CurrentFraze].Audio;
            AudioSource.Play();
        }

        while (value < 1)
        {
            value += Time.unscaledDeltaTime;

            if (changeBgm)
            {
                AudioSource.volume = value;
            }

            CG.color = Color.white * 0.85f * value;
            yield return new WaitForEndOfFrame();
        }


        AudioSource.volume = 1;
        CG.color = Color.white * 0.85f;
    }

    private IEnumerator PrintText()
    {
        Text.text = "";
       
        foreach(char letter in Frazes[CurrentFraze].Text)
        {
            Text.text += letter;
            yield return new WaitForSecondsRealtime(0.035f);
        }

        PrintCoroutine = null;
    }

    [System.Serializable]
    public class Fraze
    {
        public Sprite CG;
        public AudioClip Audio;
       [Multiline] public string Text;
        public bool Change;
    }
}
