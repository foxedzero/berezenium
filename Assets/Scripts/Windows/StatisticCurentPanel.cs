using UnityEngine;
using UnityEngine.UI;
using static Pokazateli;

public class StatisticCurentPanel : MonoBehaviour
{
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

    private int CurrentShow = -1;

    private void OnDisable()
    {
        City._Time.HourPassed -= HourPassed;
    }

    private void OnEnable()
    {
        City._Time.HourPassed += HourPassed;
        HourPassed();
    }

    public void UpdateStorage()
    {
        EnergyHoney.text = $"Энергомёд: {(int)City._Storage._EnergyHoney}";
        Wood.text = $"Древесина: {(int)City._Storage._Wood}";
        Metal.text = $"Металл: {(int)City._Storage._Metal}";
        Berezenium.text = $"Березениум: {(int)City._Storage._Berezenium}";
        Robots.text = $"Роботы: {(int)City._Storage._Robots}";
        CyberBee.text = $"Киберпчёлы: {(int)City._Storage._CyberBee}";
        Snowrunners.text = $"Снегоходы: {(int)City._Storage._Snowrunners}";
    }

    public void UpdateDrugs()
    {
        Antisleep.text = $"Антиспячкин: {City._Storage._Antisleep}";
    }

    public void UpdateFood()
    {
        Honey.text = $"Мёд: {(int)City._Foodstream._StoredFood}";
    }

    public void UpdateDataBase()
    {
        Facilities.text = $"Здания: {City._DataBase._Facilities.Length}";

        int capacity = 0;
        foreach (Facility facility in City._DataBase._Facilities)
        {
            if (facility is Home)
            {
                capacity += (facility as Home)._MaxBearCount;
            }
        }

        BearsCount.text = $"Медведи: {City._DataBase._Bears.Length}/{capacity}";

        float averageHp = 0;
        float averageStress = 0;
        foreach (Bear bear in City._DataBase._Bears)
        {
            averageHp += bear._Health;
            averageStress += bear._Stress;
        }

        averageHp /= City._DataBase._Bears.Length;
        averageStress /= City._DataBase._Bears.Length;

        string info = $"У вас {City._DataBase._Bears.Length} медведей\nСреднее здоровье медведей: {averageHp}/10\nСтресс: {Mathf.RoundToInt(averageStress)}%";

        BearsCountTip._Info = info;
    }

    public void HourPassed()
    {
        StoredEnergy.text = $"Аккумулировано: {(int)City._Energosystem._StoredEnergy}";

        Energosystem.text = $"Электросистема: {(int)(City._Energosystem._Effectivity * 100)}%";
        Saturation.text = $"Сытость: {(int)(City._Foodstream._Saturation * 100)}%";

        UpdateDataBase();
        UpdateStorage();
        UpdateDrugs();
        UpdateFood();

        SetShow(CurrentShow);
    }
    public void SetShow(int parameter)
    {
        CurrentShow = parameter;

        string info = "";

        switch (parameter)
        {
            case 0:
                info = $"Здания:";
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is ConstructionProject)
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
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is EnergyProcuder)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as EnergyProcuder)._Producing} ед/ч";
                    }
                    else if (facility._EnergyConsume > 0)
                    {
                        consumers += $"\n{facility._ConstructInfo.Name}: {facility._EnergyConsume} ед/ч";
                    }
                }
                info += consumers + $"\n\nОбщее потребление: {City._Energosystem.CurrentConsume()} ед/ч";
                EnergosystemTip._Info = info;
                break;
            case 2:
                StoredEnergyTip._Info = $"Отложенная в запас энергия, которую можно будет использовать при необходимости.\nМаксимальный запас: {City._Energosystem._EnergyCapacity}";
                break;
            case 3:
                info = "Члены экспедиции, о которых я обязан позаботиться.\nМедведи в распоряжении:";
                foreach (Bear bear in City._DataBase._Bears)
                {
                    info += $"\n{bear._Kasta} {bear._Name}";
                }
                BearsCountTip._Info = info;
                break;
            case 4:
                info = $"Сытость напрямую влияет на работоспособность и здоровье медведей.";
                SaturationTip._Info = info;
                break;
            case 5:
                info = "Мёд - основная еда у медведей, а также важный ресурс.\n\nТекущие темпы производства:";
                string consumers5 = $"\n\nПотребление:\nМедведи: {City._Foodstream.CurrentConsume()}";
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is FoodProducer)
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
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is Factory && (facility as Factory)._ResourceProduce == CityStorage.ResourceType.EnergyHoney)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._RequiredWork / (facility._Effectivity == 0 ? Mathf.Infinity : facility._Effectivity)}";
                    }
                    if (facility is EnergyProcuder && (facility as EnergyProcuder)._Resource == CityStorage.ResourceType.EnergyHoney)
                    {
                        consumers2 += $"\n{facility._ConstructInfo.Name}: {(facility as EnergyProcuder)._ConsumeLimit}";
                    }
                    if (facility._Heater == 1)
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
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is Assimilator && (facility as Assimilator)._ConstructInfo.MiningResource == CityStorage.ResourceType.Wood)
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
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is Assimilator && (facility as Assimilator)._ConstructInfo.MiningResource == CityStorage.ResourceType.Metal)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Assimilator)._Producing}";
                    }
                    if (facility is Factory && (facility as Factory)._RequiredResource == CityStorage.ResourceType.Metal)
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
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is Assimilator && (facility as Assimilator)._ConstructInfo.MiningResource == CityStorage.ResourceType.Berezenium)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Assimilator)._Producing}";
                    }
                    if (facility is EnergyProcuder && (facility as EnergyProcuder)._Resource == CityStorage.ResourceType.Berezenium)
                    {
                        consumers4 += $"\n{facility._ConstructInfo.Name}: {(facility as EnergyProcuder)._ConsumeLimit}";
                    }
                    if (facility is Factory && (facility as Factory)._RequiredResource == CityStorage.ResourceType.Berezenium)
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

                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is Factory && (facility as Factory)._ResourceProduce == CityStorage.ResourceType.Robots)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._RequiredWork / (facility._Effectivity == 0 ? Mathf.Infinity : facility._Effectivity)}";
                    }
                    if (facility is Assimilator && (facility as Assimilator)._UseRobots)
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
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is Factory && (facility as Factory)._ResourceProduce == CityStorage.ResourceType.Snowrunners)
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {(facility as Factory)._RequiredWork / (facility._Effectivity == 0 ? Mathf.Infinity : facility._Effectivity)}";
                    }
                }
                info += $"\n\nОтряды вылазок:";
                foreach (CitySally.Sally sally in City._CitySally._Sallies)
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
                foreach (Facility facility in City._DataBase._Facilities)
                {
                    if (facility is Farmacy )
                    {
                        info += $"\n{facility._ConstructInfo.Name}: {facility._Effectivity * CityTime._DaySection}";
                    }
                }
                AntisleepTip._Info = info;
                break;
            case 16:
                info = $"Киберпчелы - новейшая разработка, призванная заменить традиционных пчел и механизировать процесс добычи меда.\n\nТекущие темпы производства:";
                string asignments1 = "\n\nНазначения:";

                foreach (Facility facility in City._DataBase._Facilities)
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
}
