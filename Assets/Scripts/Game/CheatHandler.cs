using UnityEngine;

public class CheatHandler : MonoBehaviour
{
    private int Temp;

    private void Update()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.CheatInput))
        {
            UserInteract.AskVariants("Читы", new string[] {"Завершить игру", "Ресурсы", "Исследовать", "Установить время", "Установить общее значение медведям", "Вызвать медведя", "Достроить все здания", "Пропуск часов"}, new int[] { 0, 1,2,3,4,5, 6, 7}, SelectCheats);
        }
    }

    public void SelectCheats(int index)
    {
        switch (index)
        {
            case 0:
                UserInteract.AskVariants("Завершить игру", new string[] {"Победа", "Проигрыш"}, new int[] {0, 1}, EndGame);
                break;
            case 1:
                UserInteract.AskVariants("Тип ресурса", new string[] { "Энергомёд", "Березениум", "Металл", "Древесина", "Роботы", "Киберпчёлы", "Снегоходы", "Антиспячкин", "Мёд" }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, SelectResource);
                break;
            case 2:
                UserInteract.AskVariants("Вид исследования", new string[] { "Электроэнергия", "Отопление", "Медицина", "Путешествия", "Жилища", "Пища", "Производство", "Добыча" }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, SelectResearch);
                break;
            case 3:
                UserInteract.AskInput("Время в секундах", SetTime);
                break;
            case 4:
                UserInteract.AskVariants("Параметр медведя", new string[] { "Здоровье", "Стресс", "Усталость", "Время до спячки" }, new int[] { 0, 1, 2, 3 }, SetAllBearParameter);
                break;
            case 5: 
                UserInteract.AskVariants("Специализация медведя", new string[] { "Пасечник", "Конструктор", "Программист", "Биоинженер", "Первопроходец" }, new int[] { 1, 2, 3, 4, 5 }, InvokeBear);
                break;
            case 6:
                foreach(Facility facility in City._DataBase._Facilities)
                {
                    if(facility is ConstructionProject)
                    {
                        (facility as ConstructionProject)._WorkLeft = 0;
                    }
                }
                break;
            case 7:
                UserInteract.AskInput("Количество часов", TickTime);
                break;
        }
    }

    public void TickTime(string value)
    {
        int count = StaticTools.StringToInt(value);
        for(int i = 0; i < count; i++)
        {
            City._Time.SkipHour();
        }
    }

    public void EndGame(int index)
    {
        if(index == 1)
        {
            City._Factors.Defeat();
        }
        else
        {
            FindObjectOfType<GameEnder>().ShowPanel();
        }
    }

    public void SelectResource(int index)
    {
        Temp = index;
        UserInteract.AskInput("Прибавить количество", AddResource);
    }
    public void AddResource (string value)
    {
        float count = StaticTools.StringToFloat(value);
        switch (Temp)
        {
            case 0:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.EnergyHoney));
                City._Storage._EnergyHoney += count;
                break;
            case 1:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.Berezenium));
                City._Storage._Berezenium += count;
                break;
            case 2:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.Metal));
                City._Storage._Metal += count;
                break;
            case 3:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.Wood));
                City._Storage._Wood += count;
                break;
            case 4:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.Robots));
                City._Storage._Robots += Mathf.RoundToInt(count);
                break;
            case 5:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.CyberBee));
                City._Storage._CyberBee += Mathf.RoundToInt(count);
                break;
            case 6:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.Snowrunners));
                City._Storage._Snowrunners += Mathf.RoundToInt(count);
                break;
            case 7:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.Antisleep));
                City._Storage._Antisleep += Mathf.RoundToInt(count);
                break;
            case 8:
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Колдовство", (int)(City._Time._WorldTime / 60), count, CityStorage.ResourceType.Honey));
                City._Foodstream._StoredFood += Mathf.RoundToInt(count);
                break;
        }
    }

    public void SelectResearch(int index)
    {
        Temp = index;
        UserInteract.AskVariants("Установить уровень", new string[] {"0", "1", "2" ,"3"}, new int[] {0, 1, 2, 3}, SetResearch);
    }
    public void SetResearch(int value)
    {
        float count = 0;
        switch (value)
        {
            case 0:
                count = 0;
                break;
            case 1:
                count = 1000;
                break;
            case 2:
                count = 2000;
                break;
            case 3:
                count = 4000;
                break;
        }

        switch (Temp)
        {
            case 0:
                City._Research.SetResearch(CityResearch.ResearchType.Electricity, count);
                break;
            case 1:
                City._Research.SetResearch(CityResearch.ResearchType.Cold, count);
                break;
            case 2:
                City._Research.SetResearch(CityResearch.ResearchType.Medicine, count);
                break;
            case 3:
                City._Research.SetResearch(CityResearch.ResearchType.Travels, count);
                break;
            case 4:
                City._Research.SetResearch(CityResearch.ResearchType.Household, count);
                break;
            case 5:
                City._Research.SetResearch(CityResearch.ResearchType.Food, count);
                break;
            case 6:
                City._Research.SetResearch(CityResearch.ResearchType.Production, count);
                break;
            case 7:
                City._Research.SetResearch(CityResearch.ResearchType.Mining, count);
                break;
        }
    }

    public void SetTime(string value)
    {
        float time = StaticTools.StringToFloat(value);

        City._Time._WorldTime = time;
    }

    public void SetAllBearParameter(int indexes)
    {
        Temp = indexes;
        UserInteract.AskInput("Установить значение", SetAllBearParameter);
    }
    public void SetAllBearParameter(string value)
    {
        switch (Temp)
        {
            case 0:
                foreach(Bear bear in City._DataBase._Bears)
                {
                    bear._Health = StaticTools.StringToInt(value);
                }
                break;
            case 1:
                foreach (Bear bear in City._DataBase._Bears)
                {
                    bear._Stress = StaticTools.StringToFloat(value);
                }
                break;
            case 2:
                foreach (Bear bear in City._DataBase._Bears)
                {
                    bear._Tired = StaticTools.StringToFloat(value);
                }
                break;
            case 3:
                foreach (Bear bear in City._DataBase._Bears)
                {
                    bear._Effects[3] = StaticTools.StringToInt(value);
                }
                break;
        }
    }

    public void InvokeBear(int kasta)
    {
        Bear bear = BearObjectioner._Instance.RequestBear((Bear.Kasta)kasta, Random.Range(-100, 100), Random.Range(-100, 100));

        City._DataBase.RegisterBear(bear, false);
    }
}
