using System.Collections.Generic;
using UnityEngine;

public class CityTime : MonoBehaviour
{
    [SerializeField] private float WorldTime;

    public event SimpleVoid HourPassed = null;
    public event SimpleVoid DayChanged = null;

    private readonly int[] MonthDays = new int[12] { 31, 30, 31, 30, 31, 30, 31, 30, 31, 30, 31, 30 };
    private readonly string[] MonthNames = new string[12] { "Пробуждаря", "Медогрея", "Лапомарта", "Трутневика", "Берляндия", "Золотомёда", "Медоносеня", "Медоваря", "Лаполиза", "Топтыгина", "Берложника", "Спячника" };

    public float _WorldTime
    {
        get
        {
            return WorldTime;
        }
        set
        {
            WorldTime = value;
        }
    }
    public static float _DaySection => 0.083f;

    private void Update()
    {
        float time = WorldTime;
        WorldTime += Time.deltaTime * 4 * GlobalVariables._SecondTimeMultiplier;

        if(time % 1500 > WorldTime % 1500)
        {
            City._CityStatistics.SetControls();

            City._CityMessenger.AddMessage(new CityMessenger.CityMessage($"Наступил {1 + (int)(WorldTime / 1500)} день", $"Игра была сохранена.\nОтчёт на предыдущий день готов."));
        }
        if (time % 60 > WorldTime % 60)//Hour pass
        {
            int maximalOrder = 0;
            foreach(Facility facility in City._DataBase._Facilities)
            {
                maximalOrder = Mathf.Max(maximalOrder, facility._Order);
                facility.ResetHeated();
            }
            for(int i = -3; i <= maximalOrder; i++)
            {
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if(facility._Order == i)
                    {
                        facility.HourPassed();
                    }
                }
            }

            City._CitySally.HourPassed();
            City._Foodstream.HourPassed();
            City._Energosystem.HourPassed();
            City._CitySchedule.HourPassed();
            City._Factors.HourPassed();
            City._CityNotation.HourPassed();

            HourPassed?.Invoke();
        }
        if (time % 1500 > WorldTime % 1500) //Day pass
        {
            City._Weather.NewDay();

            DayChanged?.Invoke();

            SaveManager._Instance.Save();
        }
    }

    public void SkipHour() 
    {
        float time = WorldTime;
        WorldTime += 60;

        if (time % 1500 > WorldTime % 1500)
        {
            City._CityStatistics.SetControls();

            City._CityMessenger.AddMessage(new CityMessenger.CityMessage($"Наступил {1 + (int)(WorldTime / 1500)} день", $"Игра была сохранена.\nОтчёт на предыдущий день готов."));
        }
        if (time % 60 > WorldTime % 60)//Hour pass
        {
            int maximalOrder = 0;
            foreach (Facility facility in City._DataBase._Facilities)
            {
                maximalOrder = Mathf.Max(maximalOrder, facility._Order);
                facility.ResetHeated();
            }
            for (int i = -3; i <= maximalOrder; i++)
            {
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility._Order == i)
                    {
                        facility.HourPassed();
                    }
                }
            }

            City._CitySally.HourPassed();
            City._Foodstream.HourPassed();
            City._Energosystem.HourPassed();
            City._CitySchedule.HourPassed();
            City._Factors.HourPassed();
            City._CityNotation.HourPassed();

            HourPassed?.Invoke();
        }
        if (time % 1500 > WorldTime % 1500) //Day pass
        {
            City._Weather.NewDay();

            DayChanged?.Invoke();

            SaveManager._Instance.Save();
        }
    }

    public int GetYear()
    {
        int deltaDay = (int)(WorldTime % 1500);

        int yearDays = 0;
        foreach (int days in MonthDays)
        {
            yearDays += days;
        }

        return 2124 + deltaDay / yearDays;
    }

    public string GetDate()
    {
        int deltaDay = (int)(WorldTime % 1500);

        int month = 6;
        int day = 8;
        int weekDay = 0;

        for (int i = 0; i < deltaDay; i++)
        {
            weekDay = (weekDay + 1) % 7;
            day++;

            if (day > MonthDays[month])
            {
                day -= MonthDays[month];
                month = (month + 1) % MonthNames.Length;
            }
        }

        return $"{day} {MonthNames[month]}";
    }

    public string GetIntDate()
    {
        int deltaDay = (int)(WorldTime % 1500);

        int month = 6;
        int day = 8;
        int weekDay = 0;

        for (int i = 0; i < deltaDay; i++)
        {
            weekDay = (weekDay + 1) % 7;
            day++;

            if (day > MonthDays[month])
            {
                day -= MonthDays[month];
                month = (month + 1) % MonthNames.Length;
            }
        }

        int yearDays = 0;
        foreach(int days in MonthDays)
        {
            yearDays += days;
        }

        return $"{(day < 10 ? $"0{day}" : day)}.{(month < 10 ? $"0{month}" : month)}.{2124 + deltaDay / yearDays}";
    }
}
