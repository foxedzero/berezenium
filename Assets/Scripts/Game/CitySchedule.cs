using System.Collections.Generic;
using UnityEngine;

public class CitySchedule : MonoBehaviour
{
    [SerializeField] private Schedule[] Schedules;
    private bool Initialized = false;

    public event SimpleVoid OnChanges = null;

    public Schedule[] _Schedules => Schedules;
    public bool _Initialized => Initialized;

    public void SetUpSchedule(Schedule[] schedules)
    {
        Schedules = schedules;

        OnChanges?.Invoke();

        int hour = (int)((City._Time._WorldTime % 1500) / 60);
        for(int i = 0; i < Schedules.Length; i++)
        {
            Schedules[i]._Index = i;

            bool working = Schedules[i]._Hours[hour] == '1';

            foreach (Bear bear in Schedules[i]._Bears)
            {
                bear.FollowSchedule(working);
            }
        }
    }

    public void AddSchedule()
    {
        Schedules = StaticTools.ExpandMassive(Schedules, new Schedule());
        Schedules[Schedules.Length - 1]._Index = Schedules.Length - 1;

        OnChanges?.Invoke();
    }
    public void RemoveSchedule(Schedule schedule)
    {
        if(Schedules.Length > 1)
        {
            Schedules = StaticTools.RemoveFromMassive(Schedules, schedule);

            for(int i = 0; i < Schedules.Length; i++)
            {
                Schedules[i]._Index = i;
            }

            foreach (Bear bear in schedule._Bears)
            {
                Schedules[0].RegisterBear(bear, false);
            }

            OnChanges?.Invoke();
        }
    }

    public void HourPassed()
    {
        int hour = (int)((City._Time._WorldTime % 1500) / 60);
        foreach(Schedule schedule in Schedules)
        {
            bool working = schedule._Hours[hour] == '1';
            
            foreach (Bear bear in schedule._Bears)
            {
                bear.FollowSchedule(working);
            }
        }
    }

    private void Start()
    {
        Initialized = true;
    }
}

[System.Serializable]
public class Schedule
{
    private int Index = 0;
    private string Hours = "0000000011111111111100000";
    [SerializeField] private Bear[] Bears = new Bear[0];

    public Bear[] _Bears
    {
        get
        {
            return Bears;
        }
    }
    public string _Hours
    {
        get
        {
            return Hours;
        }
        set
        {
            Hours = value;
        }
    }
    public int _Index
    {
        get
        {
            return Index;
        }
        set
        {
            Index = value;
            foreach(Bear bear in Bears)
            {
                bear._Schedule = Index;
            }
        }
    }

    public string _SaveInfo
    {
        get
        {
            string bears = "";
            foreach (Bear bear in Bears)
            {
                bears += $"{StaticTools.IndexOf(City._DataBase._Bears, bear)};";
            }
            if (bears.EndsWith(";"))
            {
                bears = bears.Remove(bears.Length - 1);
            }

            return $"Hours({Hours})Bears({bears})";
        }
    }

    public bool _Work
    {
        get
        {
            int hour = (int)((City._Time._WorldTime % 1500) / 60);
            return _Hours[hour] == '1';
        }
    }

    public Schedule()
    {

    }
    public Schedule(string info)
    {
        Dictionary<string, string> values = StaticTools.GetParameters(info);

        Hours = values["Hours"];

        if(values["Bears"].Length == 0)
        {
            return;
        }

        string[] bears = values["Bears"].Split(";");
        Bears = new Bear[bears.Length];
        for(int i = 0; i < bears.Length; i++)
        {
            Bears[i] = City._DataBase._Bears[StaticTools.StringToInt(bears[i])];
        }
    }

    public void RegisterBear(Bear bear, bool remove)
    {
        if (remove)
        {
            Bears = StaticTools.RemoveFromMassive(Bears, bear);
        }
        else
        {
            Bears = StaticTools.ExcludingExpandMassive(Bears, bear);
            bear._Schedule = Index;
        }
    }
}
