using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Log : MonoBehaviour
{
    [SerializeField] private Text Text;
    private static Log Instance;

    private void Awake()
    {
        Instance = this;

        Application.logMessageReceived += Catch;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= Catch;
    }

    private void Catch(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                AddText($"<color=red>{condition}\n{stackTrace}</color>", 10);
                break;
            case LogType.Exception:
                AddText($"<color=red>{condition}\n{stackTrace}</color>", 10);
                break;
            case LogType.Log:
                AddText(condition);
                break;
        }
    }

    private void i_AddText(string info, float offTime)
    {
        if (Text.text != "")
        {
            Text.text += "\n";
        }

        if (Text.text.Length > 1500)
        {
            ClearText();
        }

        Text.text += info;

        StopAllCoroutines();
        StartCoroutine(ClearText(offTime));
    }

    private void i_ClearText()
    {
        StopAllCoroutines();
        Text.text = "";
    }

    public static void ClearText() => Instance.i_ClearText();

    public static void AddText(string info, float offTime) => Instance.i_AddText(info, offTime);
    public static void AddText(string info) => Instance.i_AddText(info, 5);

    private IEnumerator ClearText(float offTime)
    {
        yield return new WaitForSeconds(offTime);

        Text.text = "";
    }
}
