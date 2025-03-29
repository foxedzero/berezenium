using UnityEngine;
using WindowInterfaces;

public class ScheduleWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private GameObject ScheduleHolderPrefab;
    [SerializeField] private RectTransform Content;
    [SerializeField] private RectTransform NewSchedule;
    private ScheduleHolder[] Holders= new ScheduleHolder[0];

    public override string _Label => "Распорядок дня";

    private void Start()
    {
        UpdateList();
        City._CitySchedule.OnChanges += UpdateList;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        City._CitySchedule.OnChanges -= UpdateList;
    }

    public void UpdateList()
    {
        foreach(ScheduleHolder schedule in Holders)
        {
            Destroy(schedule.gameObject);
        }

        Schedule[] schedules = City._CitySchedule._Schedules;

        float y = 67.5f;

        Holders = new ScheduleHolder[schedules.Length];
        for(int i = 0; i < schedules.Length; i++)
        {
            ScheduleHolder holder = Instantiate(ScheduleHolderPrefab, Content).GetComponent<ScheduleHolder>();
            holder.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -y);

            holder.SetInfo(this, schedules[i]);

            Holders[i] = holder;

            y += 140;
        }

        NewSchedule.anchoredPosition = new Vector2(0, -y);

        Content.sizeDelta = new Vector2 (0, y + 200); 
    }

    public void CreateSchedule()
    {
        City._CitySchedule.AddSchedule();
    }
}
