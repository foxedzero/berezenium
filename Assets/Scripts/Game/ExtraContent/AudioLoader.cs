using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class AudioLoader : MonoBehaviour
{
    [SerializeField] private AudioClip[] UserMusic;

    public delegate void AudioClipVoid(AudioClip clip);

    public AudioClip[] _UserMusic => UserMusic;

    public void UpdateList()
    {
        DontDestroyOnLoad(gameObject);

        string path = Path.Combine(Application.dataPath, UserContent._DirectoryName);

        string[] pathes = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);

        UserMusic = new AudioClip[0];
        foreach(string file in pathes)
        {
            LoadClip(AddMusic, file);
        }
    }

    public void AddMusic(AudioClip clip)
    {
        if(clip.length > 60)
        {
            UserMusic = StaticTools.ExpandMassive(UserMusic, clip);
        }
    }

    public void LoadClip(AudioClipVoid callback, string path)
    {
        StartCoroutine(GetAudioClip(callback, path));
    }

    private IEnumerator GetAudioClip(AudioClipVoid callback, string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
               
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                myClip.name = Path.GetFileNameWithoutExtension(path);
                callback.Invoke(myClip);
            }
        }
    }
}