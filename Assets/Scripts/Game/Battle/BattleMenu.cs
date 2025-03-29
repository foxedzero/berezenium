using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMenu : MonoBehaviour
{
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject BattlePanel;
    [SerializeField] private bool Paused;
    
    public bool _Paused
    {
        get
        {
            return Paused;
        }
        set
        {
            Paused = value;
            MenuPanel.SetActive(value);
            BattlePanel.SetActive(!value);

            Time.timeScale = (!value).GetHashCode();
        }
    }

    private void Update()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.Cancel))
        {
            _Paused = !_Paused;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        UserInteract.AskVariants("", new string[] {"Покинуть бой (последствий нет)", "Выйти из игры"}, new int[] {0, 1}, Quit);
    }

    public void Quit(int index)
    {
        switch (index)
        {
            case 0:
                SceneManager.LoadScene(1);
                break;
            case 1:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }
}
