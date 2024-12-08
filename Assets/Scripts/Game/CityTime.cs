using System.Collections.Generic;
using UnityEngine;

public class CityTime : MonoBehaviour
{
    [SerializeField] private CityEnergosystem CityEnergosystem;
    [SerializeField] private CityFoodstream CityFoodstream;
    [SerializeField] private CityFactors CityFactors;
    [SerializeField] private Constructor Constructor;
    [SerializeField] private Weather Weather;
    [SerializeField] private CityHeater CityHeater;

    [SerializeField] private float Days;

    public event SimpleVoid DayChanged = null;

    private readonly int[] MonthDays = new int[12] { 31, 30, 31, 30, 31, 30, 31, 30, 31, 30, 31, 30};
    private readonly string[] MonthNames = new string[12] { "Пробуждаря", "Медогрея", "Лапомарта", "Трутневика", "Берляндия", "Золотомёда", "Медоносеня", "Медоваря", "Лаполиза", "Топтыгина", "Берложника", "Спячника" };

    public int _Month
    {
        get
        {
            int deltaDay = (int)Days;

            int month = 6;
            int day = 8;

            for (int i = 0; i < deltaDay; i++)
            {
                day++;

                if (day > MonthDays[month])
                {
                    day -= MonthDays[month];
                    month = (month + 1) % MonthNames.Length;
                }
            }

            return month;
        }
    }
    public float _Time
    {
        get
        {
            return Days;
        }
        set
        {
            Days = value;
        }
    }
    public float _DayProgress => Days - Mathf.FloorToInt(Days);

    private void Update()
    {
        int day = (int)Days;

        Days += Time.deltaTime / 180;

        if(day < (int)Days)
        {
            Constructor.BuildWorkDayPassed();
            CityFoodstream.DayPassed();
            CityEnergosystem.DayPassed();
            CityHeater.Heat();
            CityFactors.UpdateFactors();
            Weather.NewDay();
            City._Research.DayPassed();

            if (DayChanged != null)
            {
                DayChanged.Invoke();
            }

            foreach(Facility facility in City._DataBase._Facilities)
            {
                facility.GiveExperience();
            }

            if (Application.internetReachability != NetworkReachability.NotReachable && CityFactors._CitySatisfaction > 0.5f)
            {
                NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceiveData);

            }
        }
    }

    public void ReceiveData(string info)
    {
        Nto.Player player = Newtonsoft.Json.JsonConvert.DeserializeObject<Nto.Player>(info);

        if (player == null)
        {
            return;
        }

        Nto.Log log = new Nto.Log();
        log.player_name = player.name;

        Dictionary<string, string> dict = new Dictionary<string, string>();
        if (City._Factors._CitySatisfaction >= 0.95)
        {
            log.comment = $"Прошел {(int)Days - 1} день и все медведи Счастливы. {player.name} отлично справляется со своей должностью. Он получает премию в 500 лаподенег.";
            dict.Add("LapoMoney", $"{player.resources.LapoMoney} -> {player.resources.LapoMoney + 500}");
            log.resources_changed = dict;

            player.resources.LapoMoney += 500;
        }
        else
        {
            log.comment = $"Прошел {(int)Days - 1} день и все медведи довольны. {player.name} справляется со своей должностью.  Он получает премию в 250 лаподенег.";
            dict.Add("LapoMoney", $"{player.resources.LapoMoney} -> {player.resources.LapoMoney + 250}");
            log.resources_changed = dict;
            player.resources.LapoMoney += 250;
        }

        NtoServerInterface.PutPlayerData(log, player, null);
    }

    public int GetYear()
    {
        int deltaDay = (int)Days;

        int yearDays = 0;
        foreach (int days in MonthDays)
        {
            yearDays += days;
        }

        return 2124 + deltaDay / yearDays;
    }

    public string GetDate()
    {
        int deltaDay = (int)Days;

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
        int deltaDay = (int)Days;

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
