using UnityEngine;
using UnityEngine.UI;

public class MapInterface : MonoBehaviour
{
    [SerializeField] private WindowCreator WindowCreator;

    [SerializeField] private TimeEditor TimeEditor;
    [SerializeField] private GameObject[] PauseButtons;
    [SerializeField] private Text[] Texts;

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
        PauseButtons[0].SetActive(!TimeEditor._Paused);
        PauseButtons[1].SetActive(TimeEditor._Paused);

        Texts[0].color = new Color(0, 0, 0, 0.58f + 0.3f * (TimeEditor._TimeIndex == 0).GetHashCode());
        Texts[1].color = new Color(0, 0, 0, 0.58f + 0.3f * (TimeEditor._TimeIndex == 1).GetHashCode());
        Texts[2].color = new Color(0, 0, 0, 0.58f + 0.3f * (TimeEditor._TimeIndex == 2).GetHashCode());
        Texts[3].color = new Color(0, 0, 0, 0.58f + 0.3f * (TimeEditor._TimeIndex == 3).GetHashCode());
    }

    public void OpenShop()
    {
        WindowCreator.CreateWindow<ShopWindow>();
    }
    public void OpenDataBase()
    {
        WindowCreator.CreateWindow<CityDBWindow>();
    }
    public void OpenConstruction()
    {
        WindowCreator.CreateWindow<ConstractionWindow>();
    }
    public void OpenResearch()
    {
        WindowCreator.CreateWindow<ResearchWindow>();
    }
    public void OpenStorage()
    {
        WindowCreator.CreateWindow<StorageWindow>();
    }
}
