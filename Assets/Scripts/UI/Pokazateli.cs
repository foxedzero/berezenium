using UnityEngine;
using UnityEngine.UI;

public class Pokazateli : MonoBehaviour
{
    public enum ShowParameter {Facility, Energosystem, StoredEnergy, EnegryHoney, Berezenium, Robots, CyberBee, Snowrunners, Antisleep}

    [SerializeField] private CityStorage CityStorage;
    [SerializeField] private CityDataBase CityDataBase;
    [SerializeField] private CityEnergosystem CityEnergosystem;
    [SerializeField] private CityFoodstream CityFoodstream;
    [SerializeField] private CityTime CityTime;
    [SerializeField] private TimeEditor TimeEditor;

    [SerializeField] private RectTransform Panel;

    [SerializeField] private Text Facilities;
    [SerializeField] private Text Energosystem;
    [SerializeField] private Text StoredEnergy;
    [SerializeField] private Text BearsCount;
    [SerializeField] private Text Saturation;
    [SerializeField] private Text Honey;
    [SerializeField] private Text EnergyHoney;
    [SerializeField] private Text Wood;
    [SerializeField] private Text Metal;
    [SerializeField] private Text Berezenium;
    [SerializeField] private Text Robots;
    [SerializeField] private Text CyberBee;
    [SerializeField] private Text Snowrunners;
    [SerializeField] private Text Antisleep;

    [SerializeField] private Text[] WeatherInfo;
    [SerializeField] private Text TimeInfo;

    [SerializeField] private Tipper FacilitiesTip;
    [SerializeField] private Tipper EnergosystemTip;
    [SerializeField] private Tipper StoredEnergyTip;
    [SerializeField] private Tipper BearsCountTip;
    [SerializeField] private Tipper SaturationTip;
    [SerializeField] private Tipper HoneyTip;
    [SerializeField] private Tipper EnergyHoneyTip;
    [SerializeField] private Tipper WoodTip;
    [SerializeField] private Tipper MetalTip;
    [SerializeField] private Tipper BerezeniumTip;
    [SerializeField] private Tipper RobotsTip;
    [SerializeField] private Tipper CyberBeeTip;
    [SerializeField] private Tipper SnowrunnersTip;
    [SerializeField] private Tipper AntisleepTip;

    [SerializeField] private Tipper WeatherTip;
    [SerializeField] private Tipper TimeTip;

    private ShowParameter[] Show = new ShowParameter[0];

    private int CurrentShow = -1;

    private string StringDateInfo = "";

    public string _SaveInfo
    {
        get
        {
            string info = "Show(";
            foreach (ShowParameter parameter in Show)
            {
                info += parameter.GetHashCode() + ";";
            }
            if (info.EndsWith(";"))
            {
                info = info.Remove(info.Length - 1);
            }
            info += ")";

            return info;
        }
        set
        {
            string show = StaticTools.GetParameter(value, "Show");
            if(show == "")
            {
                return;
            }

            string[] info = show.Split(";");
            Show = new ShowParameter[info.Length];
            for(int i =0; i < Show.Length; i++)
            {
                Show[i] = (ShowParameter)(StaticTools.StringToInt(info[i]));
            }
        }
    }

    private void Start()
    {
        CityStorage.OnChanges += UpdateStorage;
        CityStorage.OnDrugChanges += UpdateDrugs;
        CityDataBase.OnBearChanges += UpdateDataBase;
        CityDataBase.OnFacilityChanges += UpdateDataBase;
        CityTime.HourPassed += HourPassed;
        CityFoodstream.OnChanges += UpdateFood;

        HourPassed();
    }

    private bool CheckParameter(ShowParameter parameter)
    {
        if(StaticTools.Contains(Show, parameter))
        {
            return true;
        }

        switch (parameter)
        {
            case ShowParameter.Facility:
                if(CityDataBase._Facilities.Length > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.Facility);
                    return true;
                }
                return false;
            case ShowParameter.StoredEnergy:
                if (CityEnergosystem._StoredEnergy > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.StoredEnergy);
                    return true;
                }
                return false;
            case ShowParameter.Energosystem:
                if (CityEnergosystem._Effectivity > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.Energosystem);
                    return true;
                }
                return false;
            case ShowParameter.EnegryHoney:
                if (CityStorage._EnergyHoney > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.EnegryHoney);
                    return true;
                }
                return false;
            case ShowParameter.Berezenium:
                if (CityStorage._Berezenium > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.Berezenium);
                    return true;
                }
                return false;
            case ShowParameter.Robots:
                if (CityStorage._Robots > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.Robots);
                    return true;
                }
                return false;
            case ShowParameter.CyberBee:
                if (CityStorage._CyberBee > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.CyberBee);
                    return true;
                }
                return false;
            case ShowParameter.Snowrunners:
                if (CityStorage._Snowrunners > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.Snowrunners);
                    return true;
                }
                return false;
            case ShowParameter.Antisleep:
                if (CityStorage._Antisleep > 0)
                {
                    Show = StaticTools.ExpandMassive(Show, ShowParameter.Antisleep);
                    return true;
                }
                return false;
        }

        return false;
    }

    public void SetShow(int parameter)
    {
        CurrentShow = parameter;

        string info = "";

        switch (parameter)
        {
            case 0:
                info = $"Здания:";
                foreach(Facility facility in CityDataBase._Facilities)
                {
                    if(facility is ConstructionProject)
                    {
                        info += $"\n<{(facility as ConstructionProject)._Construction.Name}> объем работы: {(facility as ConstructionProject)._WorkLeft}";
                    }
                    else
                    {
                        info += $"\n{facility._ConstructInfo.Name}";
                    }
                }
                FacilitiesTip._Info = info;
                break;
            case 1:
                info = $"Энергосистема влияет на работу зданий.\n\nТекущие темпы производства:";
                string consumers = "\n\nПотребители:";
                foreach(Facility facility in CityDataBase._Facilities)
                {
                    if(facility is EnergyProcuder)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as EnergyProcuder)._Producing} ед/ч";
                    }
                    else if(facility._EnergyConsume > 0)
                    {
                        consumers += $"\n{facility._ConstructInfo.Name}: {facility._EnergyConsume} ед/ч";
                    }
                }
                info += consumers + $"\n\nОбщее потребление: {City._Energosystem.CurrentConsume()} ед/ч" ;
                EnergosystemTip._Info = info;
                break;
            case 2:
                StoredEnergyTip._Info = $"Отложенная в запас энергия, которую можно будет использовать при необходимости.\nМаксимальный запас: {CityEnergosystem._EnergyCapacity}";
                break;
            case 3:
                if(CityDataBase._Bears.Length != 0)
                {
                    float averageHp = 0;
                    float averageStress = 0;
                    foreach (Bear bear in CityDataBase._Bears)
                    {
                        averageHp += bear._Health;
                        averageStress += bear._Stress;
                    }

                    averageHp /= CityDataBase._Bears.Length;
                    averageStress /= CityDataBase._Bears.Length;

                    info = $"Члены экспедиции, о которых я обязан позаботиться.\n\nЗдоровье (Усреднённое): {Mathf.RoundToInt(averageHp)}/10\nСтресс (Усредненный): {Mathf.RoundToInt(averageStress)}%\n\nМедведи в распоряжении:";

                    foreach (Bear bear in CityDataBase._Bears)
                    {
                        info += $"\n{bear._Name}    {bear._Kasta}   <color=red>ОЗ: {bear._Health}</color>  <color=cyan>Стресс: {Mathf.RoundToInt(bear._Stress)}%</color>";
                    }
                    BearsCountTip._Info = info;
                }
                else
                {
                    BearsCountTip._Info = $"Члены экспедиции, о которых я обязан позаботиться. \nИх надо спасти.";
                }
                break;
            case 4:
                info = $"Сытость напрямую влияет на работоспособность и здоровье медведей.";
                SaturationTip._Info = info;
                break;
            case 5:
                info = "Мёд - основная еда у медведей, а также важный ресурс.\n\nТекущие темпы производства:";
                string consumers5 = $"\n\nПотребление:\nМедведи: {CityFoodstream.CurrentConsume()}";
                foreach(Facility facility in CityDataBase._Facilities)
                {
                    if(facility is FoodProducer)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as FoodProducer)._BaseProduce * facility._Effectivity}";
                    }
                }
                info += consumers5;
                HoneyTip._Info = info;
                break;
            case 6:
                info = $"Энергомёд - электрически заряженный вариант мёда. Основной источник энергии медведей.\n\nТекущие темпы производства:";
                string consumers2 = "\n\nПотребление:";
                foreach(Facility facility in CityDataBase._Facilities)
                {
                    if(facility is Factory && (facility as Factory)._ResourceProduce == CityStorage.ResourceType.EnergyHoney)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._RequiredWork / (facility._Effectivity == 0 ? Mathf.Infinity : facility._Effectivity)}";
                    }
                    if(facility is EnergyProcuder && (facility as EnergyProcuder)._Resource == CityStorage.ResourceType.EnergyHoney)
                    {
                        consumers2 += $"\n{facility._ConstructInfo.Name}: {(facility as EnergyProcuder)._ConsumeLimit}";
                    }
                    if(facility._Heater == 1)
                    {
                        consumers2 += $"\n{facility._ConstructInfo.Name}: 1";
                    }
                    if (facility is Farmacy)
                    {
                        consumers2 += $"\n{facility._ConstructInfo.Name}: 1 за единицу антиспячкина";
                    }
                }
                info += consumers2;
                EnergyHoneyTip._Info = info;
                break;
            case 7:
                info = $"Древесина - широко распространенный органический ресурс, простой в добыче, но имеющий плохую устойчивость.\n\nТекущие темпы добычи:";
                string consumers3 = "\n\nПотребление:";
                foreach (Facility facility in CityDataBase._Facilities)
                {
                    if(facility is Assimilator && (facility as Assimilator)._ConstructInfo.MiningResource == CityStorage.ResourceType.Wood)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Assimilator)._Producing}";
                    }
                    if (facility is EnergyProcuder && (facility as EnergyProcuder)._Resource == CityStorage.ResourceType.Wood)
                    {
                        consumers3 += $"\n{facility._ConstructInfo.Name}: {(facility as EnergyProcuder)._ConsumeLimit}";
                    }
                    if (facility._Heater == 2)
                    {
                        consumers3 += $"\n{facility._ConstructInfo.Name}: 5";
                    }
                }
                info += consumers3;
                WoodTip._Info = info;
                break;
            case 8:
                info = $"Металл - многофункциональный и незаменимый ресурс, прочнее древесины, но более сложен в добыче.\n\nТекущие темпы добычи:";
                string consumers6 = "\n\nПотребление:";
                foreach (Facility facility in CityDataBase._Facilities)
                {
                    if (facility is Assimilator && (facility as Assimilator)._ConstructInfo.MiningResource == CityStorage.ResourceType.Metal)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Assimilator)._Producing}";
                    }
                    if(facility is Factory && (facility as Factory)._RequiredResource == CityStorage.ResourceType.Metal)
                    {
                        consumers6 += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._Cost} за обработку";
                    }
                }
                info += consumers6;
                MetalTip._Info = info;
                break;
            case 9:
                info = $"Березениум - недавно открытый элемент с аномальными магнитно-гравитационными свойствами. Назван в честь одноименной планеты, нуждается в детальном изучении, что является одной из важнейших целей нашей экспедиции.\n\nТекущие темпы добычи:";
                string consumers4 = "\n\nПотребление:";
                foreach (Facility facility in CityDataBase._Facilities)
                {
                    if (facility is Assimilator && (facility as Assimilator)._ConstructInfo.MiningResource == CityStorage.ResourceType.Berezenium)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Assimilator)._Producing}";
                    }
                    if (facility is EnergyProcuder && (facility as EnergyProcuder)._Resource == CityStorage.ResourceType.Berezenium)
                    {
                        consumers4 += $"\n{facility._ConstructInfo.Name}: {(facility as EnergyProcuder)._ConsumeLimit}";
                    }
                    if(facility is Factory && (facility as Factory)._RequiredResource == CityStorage.ResourceType.Berezenium)
                    {
                        consumers4 += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._Cost} за обработку";
                    }
                }
                info += consumers4;
                BerezeniumTip._Info = info;
                break;
            case 10:
                info = $"Роботы и дроны - новшества, достигнутые в ходе научно-технического прогресса. Берут часть тяжелой работы на себя, позволяя медведям заниматься более полезными вещами. Требуют своевременного обслуживания и контроля со стороны программистов.\n\nТекущие темпы производства:";
                string asignments = "\n\nНазначения:";
                
                foreach (Facility facility in CityDataBase._Facilities)
                {
                    if (facility is Factory && (facility as Factory)._ResourceProduce == CityStorage.ResourceType.Robots)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._RequiredWork / (facility._Effectivity == 0 ? Mathf.Infinity : facility._Effectivity)}";
                    }
                    if(facility is Assimilator && (facility as Assimilator)._UseRobots)
                    {
                        asignments += $"\n{facility._ConstructInfo.Name}: {(facility as Assimilator)._Robots}";
                    }
                }
                info += asignments;
                RobotsTip._Info = info;
                break;
            case 11:
                info = $"Снегоходы - полезный, но не очень популярный транспорт у медведей. В условиях заснеженной локации и дефицита ресурсов не имеют равных." +
                  $"\n\nТекущие темпы производства:";
                foreach (Facility facility in CityDataBase._Facilities)
                {
                    if (facility is Factory && (facility as Factory)._ResourceProduce == CityStorage.ResourceType.Snowrunners)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._RequiredWork / (facility._Effectivity == 0 ? Mathf.Infinity : facility._Effectivity)}";
                    }
                }
                info += $"\n\nОтряды вылазок:";
                foreach(CitySally.Sally sally in City._CitySally._Sallies)
                {
                    if (sally._Snowrunner)
                    {
                        info += $"\n{sally._Name}";
                    }
                }
                SnowrunnersTip._Info = info;
                break;
            case 15:
                info = $"Антиспячкин - лекарство от спячки, эффект от него держится около недели. По эффективности не сравнится с промышленными аналогами на нашей родине, но лучше хоть что-то, чем ничего.\n\nТекущие темпы производства:";
                foreach (Facility facility in CityDataBase._Facilities)
                {
                    if (facility is Farmacy)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {facility._Effectivity * CityTime._DaySection}";
                    }
                }
                AntisleepTip._Info = info;
                break;
            case 16:
                info = $"Киберпчелы - новейшая разработка, призванная заменить традиционных пчел и механизировать процесс добычи меда.\n\nТекущие темпы производства:";
                string asignments1 = "\n\nНазначения:";

                foreach (Facility facility in CityDataBase._Facilities)
                {
                    if (facility is Factory && (facility as Factory)._ResourceProduce == CityStorage.ResourceType.CyberBee)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._RequiredWork / (facility._Effectivity == 0 ? Mathf.Infinity : facility._Effectivity)}";
                    }
                   
                }

                CyberBeeTip._Info = info;
                break;
        }
    }

    public void UpdateStorage()
    {
        EnergyHoney.gameObject.SetActive(CheckParameter(ShowParameter.EnegryHoney));
        Berezenium.gameObject.SetActive(CheckParameter(ShowParameter.Berezenium));
        Robots.gameObject.SetActive(CheckParameter(ShowParameter.Robots));
        Snowrunners.gameObject.SetActive(CheckParameter(ShowParameter.Snowrunners));

        EnergyHoney.text = $"Энергомёд: {(int)City._Storage._EnergyHoney}";
        Wood.text = $"Древесина: {(int)City._Storage._Wood}";
        Metal.text = $"Металл: {(int)City._Storage._Metal}";
        Berezenium.text = $"Березениум: {(int)City._Storage._Berezenium}";
        Robots.text = $"Роботы: {(int)City._Storage._Robots}";
        CyberBee.text = $"Киберпчёлы: {(int)City._Storage._CyberBee}";
        Snowrunners.text = $"Снегоходы: {(int)City._Storage._Snowrunners}";

        PlacePokazatels();
    }

    public void UpdateDrugs()
    {
        Antisleep.gameObject.SetActive(CheckParameter(ShowParameter.Antisleep));

        Antisleep.text = $"Антиспячкин: {City._Storage._Antisleep}";

        PlacePokazatels();
    }

    public void UpdateFood()
    {
        Honey.text = $"Мёд: {(int)City._Foodstream._StoredFood}";

        PlacePokazatels();
    }

    public void UpdateDataBase()
    {
        Facilities.gameObject.SetActive(CheckParameter(ShowParameter.Facility));
        Facilities.text = $"Здания: {CityDataBase._Facilities.Length}";

        int capacity = 0;
        foreach (Facility facility in CityDataBase._Facilities)
        {
            if (facility is Home)
            {
                capacity += (facility as Home)._MaxBearCount;
            }
        }

        BearsCount.text = $"Медведи: {CityDataBase._Bears.Length}/{capacity}";
    }

    public void HourPassed()
    {
        StoredEnergy.gameObject.SetActive(CheckParameter(ShowParameter.StoredEnergy));
        StoredEnergy.text = $"Аккумулировано: {(int)CityEnergosystem._StoredEnergy}";

        Energosystem.gameObject.SetActive(CheckParameter(ShowParameter.Energosystem));
        Energosystem.text = $"Электросистема: {(int)(City._Energosystem._Effectivity * 100)}%";
        Saturation.text = $"Сытость: {(int)(City._Foodstream._Saturation * 100)}%";
     
        StringDateInfo = $"{CityTime.GetDate()}  {CityTime.GetYear()} года";

        WeatherInfo[0].text = $"{-City._Weather._Cold}";
        WeatherInfo[1].text = $"{(int)(City._Weather._Osadki * 100)}%";
        WeatherInfo[2].text = $"{(int)(City._Weather._Mist * 100)}%";
        WeatherTip._Info = $"Климатические условия окружающей среды.\n\nТемпература: -{City._Weather._Cold}\nОсадки: {City._Weather._Osadki * 100}%\nТуманность: {City._Weather._Mist * 100}%";

        UpdateDataBase();
        UpdateStorage();
        UpdateDrugs();
        UpdateFood();

        SetShow(CurrentShow);

        PlacePokazatels();
    }

    private void PlacePokazatels()
    {
        float y = 17.5f;

        if (Facilities.gameObject.activeSelf)
        {
            Facilities.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Energosystem.gameObject.activeSelf)
        {
            Energosystem.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (StoredEnergy.gameObject.activeSelf)
        {
            StoredEnergy.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (BearsCount.gameObject.activeSelf)
        {
            BearsCount.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Saturation.gameObject.activeSelf)
        {
            Saturation.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Honey.gameObject.activeSelf)
        {
            Honey.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (EnergyHoney.gameObject.activeSelf)
        {
            EnergyHoney.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Wood.gameObject.activeSelf)
        {
            Wood.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Metal.gameObject.activeSelf)
        {
            Metal.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Berezenium.gameObject.activeSelf)
        {
            Berezenium.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Robots.gameObject.activeSelf)
        {
            Robots.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (CyberBee.gameObject.activeSelf)
        {
            CyberBee.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Snowrunners.gameObject.activeSelf)
        {
            Snowrunners.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }
        if (Antisleep.gameObject.activeSelf)
        {
            Antisleep.rectTransform.anchoredPosition = new Vector2(0, -y);
            y += 35;
        }

        Panel.sizeDelta = new Vector2(375, y - 12.5f);
    }

    private void LateUpdate()
    {
        int time = (int)(City._Time._WorldTime % 1500);

        int h = time / 60;
        int m = time % 60;

        TimeInfo.text = $"{(int)(City._Time._WorldTime / 1500) + 1} день  {(h < 10 ? "0" : "")}{h}:{(m < 10 ? "0" : "")}{m}";

        TimeTip._Info = $"Сейчас {(h < 10 ? "0" : "")}{h}:{(m < 10 ? "0" : "")}{m}\n\nУстановленная скорость игры: {Mathf.Pow(2, TimeEditor._TimeIndex)}X\n\nДней с момента крушения: {(int)(City._Time._WorldTime / 1500) + 1}";
    }
}
