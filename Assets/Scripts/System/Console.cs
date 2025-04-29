using UnityEngine;
using System.IO;

public class Console : MonoBehaviour
{
    [SerializeField] private GameObject WindowPrefab;
    private string Log = "";
    private string CurrentLog = "";
    private static Console Instance = null;

    public event SimpleVoid OnLogUpdate = null;

    public static Console _Instance => Instance;
    public static string _Log => Instance.CurrentLog;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Application.logMessageReceived += Catch;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= Catch;
    }

    private void OnApplicationQuit()
    {
        File.WriteAllText(Path.Combine(Application.dataPath, "log.txt"), Log);
    }

    public void ClearLog()
    {
        CurrentLog = "";
        OnLogUpdate?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
          
        }
    }

    private void Catch(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                Log += (Log.Length > 0 ? "\n" : "") + $"<color=red>{condition}\n<size=16>{stackTrace}</size></color>";
                CurrentLog += (Log.Length > 0 ? "\n" : "") + $"<color=red>{condition}\n<size=16>{stackTrace}</size></color>";
                OnLogUpdate?.Invoke();
                break;
            case LogType.Exception:
                Log += (Log.Length > 0 ? "\n" : "") + $"<color=red>{condition}\n<size=16>{stackTrace}</size></color>";
                CurrentLog += (Log.Length > 0 ? "\n" : "") + $"<color=red>{condition}\n<size=16>{stackTrace}</size></color>";
                OnLogUpdate?.Invoke();
                break;
            case LogType.Log:
                Log += (Log.Length > 0 ? "\n" : "") + condition;
                CurrentLog += (Log.Length > 0 ? "\n" : "") + condition;
                OnLogUpdate?.Invoke();
                break;
        }

        if(CurrentLog.Length > 1500)
        {
            ClearLog();
        }
    }
}
