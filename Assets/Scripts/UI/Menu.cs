using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private TimeEditor TimeEditor;
    [SerializeField] private WindowCreator WindowCreator;
    [SerializeField] private Transform Player;
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
        }
    }

    private void Start()
    {
        if(PlayerPrefs.GetInt("Tutorial") == -1)
        {
            Vector3 position = Vector3.zero;
            for(int i = 0; i < 50; i++)
            {
                position = new Vector3(Random.Range(-25, 25), 0, Random.Range(-25, 25));

                if(position.magnitude > 25)
                {
                    break;
                }
            }

            if(position == Vector3.zero)
            {
                position = new Vector3(25, 0, 25);
            }

            Player.position = position;
            PlayerPrefs.SetInt("Tutorial", 0);
            PlayerPrefs.Save();

            Help();
        }
    }

    private void Update()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.Cancel))
        {
            _Paused = !Paused;
        }
    }

    public void Help()
    {
        _Paused = false;
        WindowCreator.CreateWindow<TutorialWindow>();
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
                FindObjectOfType<SaveManager>().Save(ToMainMenu);
                break;
            case 1:
                FindObjectOfType<SaveManager>().Save(ExitGame);
                break;
        }
    }
    public void ToMainMenu(string info)
    {
        SceneManager.LoadScene(0);
    }
    public void ExitGame(string info)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
