using UnityEngine;
using UnityEngine.UI;

public class MapInterface : MonoBehaviour
{
    [SerializeField] private WindowCreator WindowCreator;

    [SerializeField] private TimeEditor TimeEditor;
    [SerializeField] private GameObject[] PauseButtons;
    [SerializeField] private GameObject[] ResumeButtons;
    [SerializeField] private Text[] Texts;
    [SerializeField] private Text[] SallyTimeTexts;

    private void Start()
    {
        TimeEditor.OnTimeUpdate += UpdateTime;
        UpdateTime();
    }

    public void SetPause(bool state)
    {
        TimeEditor._Paused = state;
    }
    public void SetTime(int index)
    {
        TimeEditor._TimeIndex = index;
    }
    public void UpdateTime()
    {
        foreach(GameObject gameObject in PauseButtons)
        {
            gameObject.SetActive(!TimeEditor._Paused);
        }
        foreach (GameObject gameObject in ResumeButtons)
        {
            gameObject.SetActive(TimeEditor._Paused);
        }

        Texts[0].color = new Color(1, 1, 1, 0.25f + 0.55f * (TimeEditor._TimeIndex == 0).GetHashCode());
        Texts[1].color = new Color(1, 1, 1, 0.25f + 0.55f * (TimeEditor._TimeIndex == 1).GetHashCode());
        Texts[2].color = new Color(1, 1, 1, 0.25f + 0.55f * (TimeEditor._TimeIndex == 2).GetHashCode());
        Texts[3].color = new Color(1, 1, 1, 0.25f + 0.55f * (TimeEditor._TimeIndex == 3).GetHashCode());

        SallyTimeTexts[0].color = new Color(1, 1, 1, 0.25f + 0.75f * (TimeEditor._TimeIndex == 0).GetHashCode());
        SallyTimeTexts[1].color = new Color(1, 1, 1, 0.25f + 0.75f * (TimeEditor._TimeIndex == 1).GetHashCode());
        SallyTimeTexts[2].color = new Color(1, 1, 1, 0.25f + 0.75f * (TimeEditor._TimeIndex == 2).GetHashCode());
        SallyTimeTexts[3].color = new Color(1, 1, 1, 0.25f + 0.75f * (TimeEditor._TimeIndex == 3).GetHashCode());
    }

    public void OpenSchedule()
    {
        WindowCreator.CreateWindow<ScheduleWindow>();
    }

    public void OpenDataBase()
    {
        WindowCreator.CreateWindow<CityDBWindow>();
    }
    public void OpenConstruction()
    {
        WindowCreator.CreateWindow<ConstractionWindow>();

        NewTutorialSystem.Instance.OpenedBuildWindow();
    }
    public void OpenResearch()
    {
        WindowCreator.CreateWindow<TechnologyWindow>();
    }
    public void OpenStorage()
    {
        WindowCreator.CreateWindow<StatisticWindow>();
    }
}
