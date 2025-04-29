using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameEnder : MonoBehaviour
{
    [SerializeField] private GameObject CongratsPanel;
    [SerializeField] private CityFactors CityFactors;
    [SerializeField] private Image BlackScreen;
    [SerializeField] private bool GameEnd;

    [SerializeField] private TimeEditor TimeEditor;

    [SerializeField] private Musician.MusicOrder WinMusic;

    public bool _GameEnd
    {
        get
        {
            return GameEnd;
        }
        set
        {
            GameEnd = value;
        }
    }

    public void ShowPanel()
    {
        FindObjectOfType<Musician>().SetMusic(WinMusic, false);
        CongratsPanel.SetActive(true);
        TimeEditor._Paused = true;
        GameEnd = true;
        PlayerPrefs.SetInt("PlayerWinrate", PlayerPrefs.GetInt("PlayerWinrate") + 1);
        PlayerPrefs.Save();
    }

    public void End(int scene)
    {
        StopAllCoroutines();
        StartCoroutine(Ending(scene));
    }

    private IEnumerator Ending(int scene)
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        float a = 0;
        while(a < 1)
        {
            a += Time.unscaledDeltaTime * 0.75f;
            BlackScreen.color = new Color(0, 0, 0, a);
            yield return waitForEndOfFrame;
        }

        SceneManager.LoadScene(scene);
    }
}
