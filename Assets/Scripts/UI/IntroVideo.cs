using System.Collections;
using UnityEngine;

public class IntroVideo : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusicSource;

    IEnumerator waitForEndNumerator()
    {
        yield return new WaitForSeconds(56);
        closeVideo();

    }

    private void closeVideo()
    {
        StopAllCoroutines();
        backgroundMusicSource.mute = false;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("isFirstLaunch") == 1) { gameObject.SetActive(false); return; }
        backgroundMusicSource.mute = true;
        StartCoroutine(waitForEndNumerator());
        PlayerPrefs.SetInt("isFirstLaunch",1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { closeVideo(); }
    }
}
