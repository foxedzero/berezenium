using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private Text Text;
    [SerializeField]
    [Multiline] private string info;

    [SerializeField][Multiline] private string[] Tips;
    private int CurrentTip = 0;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Tutorial") == -1)
        {
            Panel.SetActive(true);
        }

        CurrentTip = PlayerPrefs.GetInt("TutorialTip");
        ShowTip();

        info = "";

        foreach(string tip in Tips)
        {
            info += $"\n\n{tip}";
        }
    }

    private void Update()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.ShowTutorial))
        {
            ToggleShow();
        }
        if (!Panel.activeSelf)
        {
            return;
        }

        if (InputManager.GetButtonDown(InputManager.ButtonEnum.NextTip))
        {
            Next(false);
        }
        else if (InputManager.GetButtonDown(InputManager.ButtonEnum.PrevTip))
        {
            Next(true);
        }
    }

    public void ToggleShow()
    {
        if (Panel.activeSelf)
        {
            PlayerPrefs.SetInt("Tutorial", 0);
            Panel.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetInt("Tutorial", -1);
            Panel.SetActive(true);
        }
        PlayerPrefs.Save();
    }

    public void Next(bool previous)
    {
        if (previous)
        {
            CurrentTip--;
            if (CurrentTip < 0)
            {
                CurrentTip = 0;
            }

            PlayerPrefs.SetInt("TutorialTip", CurrentTip);
            PlayerPrefs.Save();

            ShowTip();
        }
        else
        {
            CurrentTip++;
            if (CurrentTip >= Tips.Length)
            {
                CurrentTip = Tips.Length - 1;
                PlayerPrefs.SetInt("Tutorial", 0);
                Panel.SetActive(false);
            }

            PlayerPrefs.SetInt("TutorialTip", CurrentTip);
            PlayerPrefs.Save();

            ShowTip();
        }
    }

    public void ShowTip()
    {
       Text.text = Tips[CurrentTip];
    }
}
