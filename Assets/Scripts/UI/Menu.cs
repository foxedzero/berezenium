using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour, ICancelable
{
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private TimeEditor TimeEditor;
    [SerializeField] private Weather Weather;
    [SerializeField] private Text SeedInfo;
    private bool Paused = false;

    private NeedCursorOrder NeedCursor = new NeedCursorOrder();

    public bool _Paused
    {
        get
        {
            return Paused;
        }
        set
        {
            Paused = value;

            MenuPanel.SetActive(Paused);

            CursorManager.SetNeedMouse(NeedCursor, !Paused);

            TimeEditor._Menued = value;

            Weather.FreezeSnow(gameObject, !Paused);
        }
    }

    private void Start()
    {
        CancelQueue.Register(this, false);

        SeedInfo.text = $"Seed: {SaveManager._Seed}";
    }

    public void CopySeed()
    {
        GUIUtility.systemCopyBuffer = SaveManager._Seed;
    }

    public void Cancel()
    {
        _Paused = !Paused;
    }

    public void ExitGame()
    {
        UserInteract.AskVariants("Игра перед выходом сохранится", new string[] {"В главное меню", "На рабочий стол"}, new int[] { 0, 1}, ExitGame);
    }

    public void ExitGame(int index)
    {
        switch (index)
        {
            case 0:
                FindObjectOfType<SaveManager>().Save();
                ToMainMenu();
                break;
            case 1:
                FindObjectOfType<SaveManager>().Save();
                FullExitGame();
                break;
        }
    }
    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void FullExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
