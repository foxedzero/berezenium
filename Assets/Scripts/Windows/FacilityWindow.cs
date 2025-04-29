
using UnityEngine;
using UnityEngine.UI;
using static CityResearch;

public class FacilityWindow : DefaultWindow
{
    [SerializeField] private GameObject ParameterPrefab;
    [SerializeField] private GameObject BearSlotPrefab;
    [SerializeField] private RectTransform RobotButton;
    [SerializeField] private RectTransform CyberBeeButton;
    [SerializeField] private RectTransform AddButton;
    [SerializeField] private Image Icon;

    [SerializeField] private RectTransform AutoAssignButton;
    [SerializeField] private RectTransform UnassignButton;
    [SerializeField] private RectTransform HeaterButton;
    [SerializeField] private RectTransform ResearchButton;
    [SerializeField] private RectTransform ConsumeLimitButton;
    [SerializeField] private RectTransform UseRobotsButton;
    [SerializeField] private RectTransform ChangeProduceButton;
    [SerializeField] private RectTransform ChangeHeatLevelButton;
    [SerializeField] private RectTransform FlyDurandalButton;

    [SerializeField] private Text[] ParameterInfos;
    [SerializeField] private Tipper[] ParameterTips;

    [SerializeField] private Text Describtion;

    [SerializeField] private RectTransform PersonalContent;
    private BearSlot[] BearSlots = new BearSlot[0];
    [SerializeField] private RectTransform ParameterContent;
    [SerializeField] private Text PersonalLabel;

    [SerializeField] private Facility Facility;

    private Bear SelectedBear = null;
    private int[] Configuration = new int[0];
    //0 - Спец. 
    //1 - Эффективность
    //2 - Электропот
    //3 - хладостойк.
    //4 - коэф уставания
    //5 - уменьш. стреса
    //6 - электроёмкость
    //7 - режим работы с роботами или без
    //8 - произв ресурс
    //9 - итог работы
    //10 - осталось работы
    //11 - требуем ресурс
    //12 - Лимит топлива
    //13 - мощность лаборатории
    //14 - шанс лечения
    //15 - оставшееся ресурсы
    //16 - требуемая работа
    //17 - количество роботов
    //18 - отопитель
    //19 - обогреватель

    public override string _Label
    {
        get
        {
            if(Facility == null)
            {
                return "";
            }

            if(Facility is ConstructionProject)
            {
                return $"Проект \"{(Facility as ConstructionProject)._Construction.Name}\" #{StaticTools.IndexOf(City._DataBase._Facilities, Facility)}";
            }
            else
            {
                return $"{Facility._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Facility)}";
            }
        }
    }

    public Facility _Facility => Facility;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if(Facility != null)
        {
            Facility.OnSmthChange -= UpdateInfo;
            Facility.OnFacilityDestroy -= Close;
        }
    }

    public void SetInfo(Facility facility)
    {
        if (Facility != null)
        {
            Facility.OnSmthChange -= UpdateInfo;
            Facility.OnFacilityDestroy -= Close;
        }

        Facility = facility;

        AutoAssignButton.gameObject.SetActive(true);
        UnassignButton.gameObject.SetActive(true);
        HeaterButton.gameObject.SetActive(false);
        ResearchButton.gameObject.SetActive(false);
        ConsumeLimitButton.gameObject.SetActive(false);
        UseRobotsButton.gameObject.SetActive(false);
        ChangeProduceButton.gameObject.SetActive(false);
        ChangeHeatLevelButton.gameObject.SetActive(false);
        FlyDurandalButton.gameObject.SetActive(false);

        foreach (Window window in Lister._Windows)
        {
            if (window is FacilityWindow && this != window)
            {
                if ((window as FacilityWindow).Facility == Facility)
                {
                    window.Close();
                    break;
                }
            }
        }

        Facility.OnSmthChange += UpdateInfo;
        Facility.OnFacilityDestroy += Close;

        Configuration = new int[0] {};
        if(facility is Home)
        {
            Configuration = new int[] {2, 3, 4, 5};

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if(facility is EnergyProcuder)
        {
            Configuration = new int[] {0, 1, 3, 4, 6, 9, 11, 12, 16};

            if(City._Research.GetResearchLevel(ResearchType.Cold) >= 2)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 18);
            }

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            ConsumeLimitButton.gameObject.SetActive(true);
            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if(facility is Accumulator)
        {
            Configuration = new int[] {6};
            AutoAssignButton.gameObject.SetActive(false);
            UnassignButton.gameObject.SetActive(false);
        }
        else if (facility is OutHeater)
        {
            Configuration = new int[] { 11, 12 };
            AutoAssignButton.gameObject.SetActive(false);
            UnassignButton.gameObject.SetActive(false);
            ChangeHeatLevelButton.gameObject.SetActive(true);
        }
        else if (facility is CyberPaseka)
        {
            Configuration = new int[] { 0, 1, 2, 3, 4, 9, 16, 17 };

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if (facility is FoodProducer)
        {
            Configuration = new int[] { 0, 1, 2, 3, 4, 9};

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if(facility is ConstructionProject)
        {
            Configuration = new int[] { 0, 1, 2, 3, 4, 10};

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if(facility is Assimilator)
        {
            Configuration = new int[] { 0, 1, 2, 3, 4, 7, 8, 9, 17, 16 };

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            UseRobotsButton.gameObject.SetActive(true);
            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if (facility is Factory)
        {
            Configuration = new int[] { 0, 1, 2, 3, 4, 8, 10, 11 };

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
            ChangeProduceButton.gameObject.SetActive((facility as Factory)._MechFactory);
        }
        else if (facility is Laboratory)
        {
            Configuration = new int[] { 0, 1, 2, 3, 4, 13, 15};

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            ResearchButton.gameObject.SetActive(true);
            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if (facility is MedFacility)
        {
            Configuration = new int[] { 0, 1, 2, 3, 4,  10, 14};

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if(facility is Farmacy)
        {
            Configuration = new int[] { 0, 1, 2, 3, 4, 8, 10, 11 };

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }

            HeaterButton.gameObject.SetActive(City._Research.GetResearchLevel(ResearchType.Cold) >= 1);
        }
        else if (facility is Finders)
        {
            Configuration = new int[] { 0, 1, 2, 3, 10};

            if (City._Research.GetResearchLevel(ResearchType.Cold) >= 1)
            {
                Configuration = StaticTools.ExpandMassive(Configuration, 19);
            }
        }
        else if (facility is Durandal)
        {
            Configuration = new int[] { 0, 3 };
            FlyDurandalButton.gameObject.SetActive(true);
        }

        foreach (Text info in ParameterInfos)
        {
            Destroy(info.gameObject);
        }

        ParameterInfos = new Text[Configuration.Length];
        ParameterTips = new Tipper[Configuration.Length];
        for (int i = 0; i < Configuration.Length; i++)
        {
            GameObject paramter = Instantiate(ParameterPrefab, ParameterContent);
            ParameterInfos[i] = paramter.GetComponent<Text>();
            ParameterTips[i] = paramter.GetComponent<Tipper>();

            paramter.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,  -30 * i);
        }

        float y = 182.5f;
        if (AutoAssignButton.gameObject.activeSelf)
        {
            AutoAssignButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }
        if (UnassignButton.gameObject.activeSelf)
        {
            UnassignButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }
        if (HeaterButton.gameObject.activeSelf)
        {
            HeaterButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }
        if (ResearchButton.gameObject.activeSelf)
        {
            ResearchButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }
        if (ConsumeLimitButton.gameObject.activeSelf)
        {
            ConsumeLimitButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }
        if (UseRobotsButton.gameObject.activeSelf)
        {
            UseRobotsButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }
        if (ChangeProduceButton.gameObject.activeSelf)
        {
            ChangeProduceButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }
        if (ChangeHeatLevelButton.gameObject.activeSelf)
        {
            ChangeHeatLevelButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }
        if (FlyDurandalButton.gameObject.activeSelf)
        {
            FlyDurandalButton.anchoredPosition = new Vector2(90, y);
            y -= 50;
        }

        ParameterContent.sizeDelta = new Vector2(0, 30 * Configuration.Length);

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        if (Facility == null)
        {
            Destroy(gameObject);
            return;
        }

        if (Facility is ConstructionProject)
        {
            Icon.sprite = (Facility as ConstructionProject)._Construction.Icon;
        }
        else
        {
            Icon.sprite = Facility._ConstructInfo.Icon;
        }

        Label.text = _Label;

        for (int i = 0; i < Configuration.Length; i++)
        {
            switch (Configuration[i])
            {
                case 0:
                    ParameterInfos[i].text = $"Специальность: {Facility._RequiredKasta}";
                    ParameterTips[i]._Info = $"Специализация медведя, который будет назначен здесь работать\r\n\r\nМедведь с иной специализацией работает в два раза хуже";
                    break;
                case 1:
                    {
                        ParameterInfos[i].text = $"Эффективность: {(int)(Facility._Effectivity * 100)}%";

                        string info = "Эффективность определяет результат работы здания.\nПодсчитывается как суммарная работа медведей, помноженная на факторы.\n\nМедведи:";
                        foreach (Bear bear1 in Facility._AssignedBears)
                        {
                            info += $"\n{bear1._Name}  {(int)(bear1._Work * 100f)}%";
                        }
                        info += "\n\nФакторы:";
                        foreach (CityFactors.Factor factor in Facility.GetEffectivityFactors())
                        {
                            info += $"\n{factor}";
                        }

                        info += $"\n\nИтоговое значение: {Facility._Effectivity}";

                        ParameterTips[i]._Info = info;
                    }
                    break;
                case 2:
                    ParameterInfos[i].text = $"Требуемое электричество: {Facility._EnergyConsume} ({Facility._BaseEnergyConsume}) ед/ч";
                    ParameterTips[i]._Info = $"Электричество, которое требуется для нормальной работы здания.\n\nБазовое требуемое электричество: {Facility._BaseEnergyConsume}  ед/ч\nМинимальный коэффициент работы от электричества: {(int)(Facility._MinimalEnergyCoefficiente * 100)}%";
                    break;
                case 3:
                    ParameterInfos[i].text = $"Хладостойкость: {Facility._ColdEndurance}";
                    ParameterTips[i]._Info = $"Хладостойкость противостоит холоду. Уровень тепла медведя повышается на хладостойкость здания.{(Facility._CanBeHeated ? $"\n\nЭффект отопления: {Facility._Heated}" : "")}";
                    break;
                case 4:
                    if(Facility is Home)
                    {
                        ParameterInfos[i].text = $"Расслабление медведя: {-(int)(Facility._Tiring * 100 * CityTime._DaySection)} %/ч";
                        ParameterTips[i]._Info = $"Усталость, которая будет уменьшаться у медведя с каждым часом отдыха.";
                    }
                    else
                    {
                        ParameterInfos[i].text = $"Уставание медведя: {(int)(Facility._Tiring * 100 * CityTime._DaySection)} %/ч";
                        ParameterTips[i]._Info = $"Усталость, которая будет накапливаться у медведя с каждым часом работы.";
                    }
                    break;
                case 5:
                    ParameterInfos[i].text = $"Уменьшение стресса: {(int)((Facility as Home)._StressDown * 10) / 10f} %/ч";
                    ParameterTips[i]._Info = $"Понижение стресса медведя, при нахождении его в здании.\nБазовое понижение: {(int)((Facility as Home)._BaseStressDown * 10)/10f}%";
                    break;
                case 6:
                    if(Facility is Accumulator)
                    {
                        ParameterInfos[i].text = $"Электроёмкость: {(Facility as Accumulator)._Capacity}";
                        ParameterTips[i]._Info = $"Количество электричества, которое сможет сохранить аккумулятор.";
                    }
                    else if(Facility is EnergyProcuder)
                    {
                        ParameterInfos[i].text = $"Электроёмкость: {(Facility as EnergyProcuder)._EnergyCapacity}";
                        ParameterTips[i]._Info = $"Количество электричества, которое сможет сохранить электростанция.\nКонечно, аккумуляторы смогут сохранить больше.";
                    }
                    break;
                case 7:
                    ParameterInfos[i].text = $"Режим работы: {((Facility as Assimilator)._UseRobots ? "Роботизированный" : "Ручной")}";
                    ParameterTips[i]._Info = $"Метод работы здания. В роботизированном режиме для работы потребуются роботы и программисты. В ручном работать может каждый медведь.";
                    break;
                case 8:
                    if(Facility is Assimilator)
                    {
                        ParameterInfos[i].text = $"Добываемый ресурс: {CityStorage.ResourceName((Facility as Assimilator)._ConstructInfo.MiningResource)}";
                        ParameterTips[i]._Info = $"Ресурс, который будет добыт в ходе работы.";
                    }
                    else if(Facility is Farmacy)
                    {
                        ParameterInfos[i].text = $"Производимый препарат: Антиспячкин";
                        ParameterTips[i]._Info = $"Препарат, который будет произведён по окончании работы.\nТребуется работы для одного препарата: {(Facility as Farmacy)._RequiredWork}";
                    }
                    else if(Facility is Factory)
                    {
                        ParameterInfos[i].text = $"Производимый ресурс: {CityStorage.ResourceName((Facility as Factory)._ResourceProduce)}";
                        ParameterTips[i]._Info = $"Ресурс, который будет произведён по окончании работы.\nТребуется работы на единицу ресурса: {(Facility as Factory)._RequiredWork}";
                    }
                    break;
                case 9:
                    if(Facility is Assimilator)
                    {
                        ParameterInfos[i].text = $"Объем добычи: {Mathf.RoundToInt((Facility as Assimilator)._Producing)} ед/ч";

                        if((Facility as Assimilator)._UseRobots)
                        {
                            ParameterTips[i]._Info = $"Сколько ресурсов будет получено в следующий час.\n\nНоминальное количество ресурса: {(Facility as Assimilator).GetNominal()} ед/ч\nИтоговое значение: {(Facility as Assimilator).GetNominal() / 2f} + {(Facility as Assimilator).GetNominal() / 2f} * {Facility._Effectivity} = {(Facility as Assimilator)._Producing} ед/ч";
                        }
                        else
                        {
                            ParameterTips[i]._Info = $"Сколько ресурсов будет получено в следующий час.\n\nНоминальное количество ресурса: {(Facility as Assimilator).GetNominal()} ед/ч";
                        }
                    }
                    else if(Facility is EnergyProcuder)
                    {
                        ParameterInfos[i].text = $"Энерговыработка: {Mathf.RoundToInt((Facility as EnergyProcuder)._Producing)} ед/ч";
                        ParameterTips[i]._Info = $"Сколько электричества будет произведено в следующий час.\n\nНоминальное количества электричества: {(Facility as EnergyProcuder)._BaseProduce} ед/ч\nИтоговое значение: {(Facility as EnergyProcuder)._BaseProduce / 2f} + {(Facility as EnergyProcuder)._BaseProduce / 2f} * {Facility._Effectivity}^2 = {(Facility as EnergyProcuder)._Producing} ед/ч";
                    }
                    else if (Facility is FoodProducer)
                    {
                        ParameterInfos[i].text = $"Производство мёда: {Mathf.RoundToInt((Facility as FoodProducer)._BaseProduce * Facility._Effectivity)} ед/ч";
                        ParameterTips[i]._Info = $"Сколько мёда будет произведено в следующий час.\n\nНоминальное количество мёда: {(Facility as FoodProducer)._BaseProduce} ед/ч";
                    }
                    break;
                case 10:
                    if(Facility is ConstructionProject)
                    {
                        ParameterInfos[i].text = $"Осталось работы: {(int)((Facility as ConstructionProject)._WorkLeft * 100)}%";
                        ParameterTips[i]._Info = $"Количество работы до конца стройки.\n\nСнижение работы: {(int)(Facility._Effectivity * CityTime._DaySection * 100)}%";
                    }
                    else if(Facility is Factory)
                    {
                        ParameterInfos[i].text = $"Осталось работы: {(int)((Facility as Factory)._Work * 100)}%";
                        ParameterTips[i]._Info = $"Количество работы до производства ресурса.\n\nСнижение работы: {(int)(Facility._Effectivity * CityTime._DaySection  * 100)}%";
                    }
                    else if(Facility is Farmacy)
                    {
                        ParameterInfos[i].text = $"Осталось работы: {(int)((Facility as Farmacy)._Work * 100)}%";
                        ParameterTips[i]._Info = $"Количество работы до производства препарата.\n\nСнижение работы: {(int)(Facility._Effectivity * CityTime._DaySection * 100)}%";
                    }
                    else if (Facility is Finders)
                    {
                        ParameterInfos[i].text = $"Осталось работы: {(int)((Facility as Finders)._FindWork * 100)}%";
                        ParameterTips[i]._Info = $"Количество работы до обнаружение следующей точки падения.\n\nСнижение работы: {(int)(Facility._Effectivity * CityTime._DaySection * 100)}%";
                    }
                    else if (Facility is MedFacility)
                    {
                        ParameterInfos[i].text = $"Время для лечения: {(Facility as MedFacility)._HealHours} ч";

                        string info = "Количество работы до возможного лечения пациента.\n\n";
                        for(int ii = 0; ii < (Facility as MedFacility)._AssignedBears.Length; ii++)
                        {
                            info += $"{Facility._AssignedBears[ii]._Name}: {(Facility as MedFacility)._BearWork[ii]} ч";
                        }

                        ParameterTips[i]._Info = info;
                    }
                    break;
                case 11:
                    if (Facility is Factory)
                    {
                        ParameterInfos[i].text = $"Требуемый материал: {CityStorage.ResourceName((Facility as Factory)._RequiredResource)} ({(Facility as Factory)._Cost} ед)";
                        ParameterTips[i]._Info = $"Материал, который нужен для производства ресурса.\n\nМатериал: {CityStorage.ResourceName((Facility as Factory)._RequiredResource)}\nКоличество: {(Facility as Factory)._Cost}";
                    }
                    else if (Facility is Farmacy)
                    {
                        ParameterInfos[i].text = $"Требуемый материал: {CityStorage.ResourceName((Facility as Farmacy)._RequiredResource)} ({(Facility as Farmacy)._Cost} ед)";
                        ParameterTips[i]._Info = $"Материал, который нужен для производства препарата.\n\nМатериал: {CityStorage.ResourceName((Facility as Farmacy)._RequiredResource)}\nКоличество: {(Facility as Farmacy)._Cost}";
                    }
                    else if(Facility is EnergyProcuder)
                    {
                        ParameterInfos[i].text = $"Вид топлива: {CityStorage.ResourceName((Facility as EnergyProcuder)._Resource)}";
                        ParameterTips[i]._Info = $"Материал, который будет использоваться в качестве топлива.\n\nТопливо: {CityStorage.ResourceName((Facility as EnergyProcuder)._Resource)}\nКоличество: {(Facility as EnergyProcuder)._ConsumeLimit} ед/ч";
                    }
                    else if (Facility is OutHeater)
                    {
                        ParameterInfos[i].text = $"Вид топлива: Энергомёд ({(Facility as OutHeater)._Consume} ед/ч)";
                        ParameterTips[i]._Info = $"Ресурс, который будет использоваться в качестве топлива.\n\nТопливо: Энергомёд\nКоличество: {(Facility as OutHeater)._Consume} ед/ч";
                    }
                    break;
                case 12:
                    if(Facility is EnergyProcuder)
                    {
                        ParameterInfos[i].text = $"Лимит топлива: {(int)((Facility as EnergyProcuder)._ConsumeLimit * 10) / 10f} ед/ч";
                        ParameterTips[i]._Info = $"Количество топлива, сжигаемого в час для выработки электричества.\nЧем больше сжигается топлива, тем больше вырабатывается энергии.";
                    }
                    else if(Facility is OutHeater)
                    {
                        ParameterInfos[i].text = $"Номинал тепла: {(Facility as OutHeater)._HeatLevel} ед";
                        ParameterTips[i]._Info = $"Количество тепла, к которому будет стремиться отопитель.\nЧем больше номинал, тем больше требуется топлива и меньше КПД.";
                    }
                    break;
                case 13:
                    ParameterInfos[i].text = $"Изучение: {(int)((Facility as Laboratory)._ResearchPoints * Facility._Effectivity)} ед/ч";
                    ParameterTips[i]._Info = $"Очки, которые пойдут в ход исследования выбранной технологии.\n\nНоминальное изучение: {(Facility as Laboratory)._ResearchPoints} ед/ч";
                    break;
                case 14:
                    {
                        ParameterInfos[i].text = $"Средний шанс лечения: {(Facility as MedFacility).AverageChance()}%";

                        string info = $"Шанс медика вылечить медведя:";
                        foreach (Bear bear in Facility._Bears)
                        {
                            info += $"\n{bear._Name}: {(int)(15 * bear._Work)}%";
                        }

                        ParameterTips[i]._Info = info;
                    }
                    break;
                case 15:
                    string research = "";
                    switch ((Facility as Laboratory)._Target)
                    {
                        case CityResearch.ResearchType.Electricity:
                            research = $"Электроэнергия";
                            break;
                        case CityResearch.ResearchType.Cold:
                            research = $"Отопительные системы";
                            break;
                        case CityResearch.ResearchType.Medicine:
                            research = $"Медицина и препараты";
                            break;
                        case CityResearch.ResearchType.Travels:
                            research = $"Путешествия и логистика";
                            break;
                        case CityResearch.ResearchType.Household:
                            research = $"Жилищные условия";
                            break;
                        case CityResearch.ResearchType.Food:
                            research = $"Пасеки и мёд";
                            break;
                        case CityResearch.ResearchType.Production:
                            research = $"Производство";
                            break;
                        case CityResearch.ResearchType.Mining:
                            research = $"Добыча";
                            break;
                    }

                    ParameterInfos[i].text = $"Исследование: {research}";
                    ParameterTips[i]._Info = $"Выбранный курс разработок для данной лаборатории.";
                    break;
                case 16:
                    if(Facility is EnergyProcuder)
                    {
                        ParameterInfos[i].text = $"Требуемая работа: {Mathf.RoundToInt((Facility as EnergyProcuder)._RequiredWork * 100)}%";
                        ParameterTips[i]._Info = $"Работа необходимая для нормальной работы здания.";
                    }
                    else if(Facility is Assimilator)
                    {
                        if((Facility as Assimilator)._UseRobots)
                        {
                            ParameterInfos[i].text = $"Требуемая работа: {Mathf.RoundToInt((Facility as Assimilator)._RequiredWork * 100)}%";
                            ParameterTips[i]._Info = $"Работа необходимая для нормальной работы здания.";
                        }
                        else
                        {
                            ParameterInfos[i].text = $"Требуемая работа: -";
                            ParameterTips[i]._Info = $"Работа необходимая для нормальной работы здания.";
                        }
                    }
                    else if (Facility is CyberPaseka)
                    {
                        ParameterInfos[i].text = $"Требуемая работа: {Mathf.RoundToInt((Facility as CyberPaseka)._RequiredWork * 100)}%";
                        ParameterTips[i]._Info = $"Работа необходимая для нормальной работы здания.";
                    }
                    break;
                case 17:
                    if(Facility is Assimilator)
                    {
                        if ((Facility as Assimilator)._UseRobots)
                        {
                            ParameterInfos[i].text = $"Роботы: {(Facility as Assimilator)._Robots}/{(Facility as Assimilator)._MaxRobots}";
                            ParameterTips[i]._Info = $"Количество назначенных на работу роботов.";
                        }
                        else
                        {
                            ParameterInfos[i].text = $"Роботы: -";
                            ParameterTips[i]._Info = $"Количество назначенных на работу роботов.";
                        }
                    }
                    else if (Facility is CyberPaseka)
                    {
                        ParameterInfos[i].text = $"Киберпчёлы: {(Facility as CyberPaseka)._CyberBees}/{(Facility as CyberPaseka)._MaxCyberBees}";
                        ParameterTips[i]._Info = $"Количество назначенных на работу киберпчёл.";
                    }
                    break;
                case 18:
                    if(Facility is EnergyProcuder)
                    {
                        ParameterInfos[i].text = $"Эффект отопления: {(Facility as EnergyProcuder)._HeatEffect}";
                        ParameterTips[i]._Info = $"На сколько единиц здания станут теплее.\n\nОхватываемый радиус: {(Facility as EnergyProcuder)._HeatRadius}";
                    }
                    else if(Facility is OutHeater)
                    {
                        ParameterInfos[i].text = $"Эффект отопления: {(Facility as OutHeater)._HeatLevel}";
                        ParameterTips[i]._Info = $"На сколько единиц здания станут теплее.\n\nОхватываемый радиус: {(Facility as OutHeater)._HeatRadius}";
                    }
                    break;
                case 19:
                    if (Facility._CanBeHeated)
                    {
                        switch (Facility._Heater)
                        {
                            case 0:
                                ParameterInfos[i].text = $"Режим обогревателя: Выключен";
                                ParameterTips[i]._Info = $"Обогреватель согревает здание за счёт сжигания энергоносителей.\n\nПотребление: 0\nЭффект отопления: 0";
                                break;
                            case 1:
                                ParameterInfos[i].text = $"Режим обогревателя: Энергомёд";
                                ParameterTips[i]._Info = $"Обогреватель согревает здание за счёт сжигания энергоносителей.\n\nПотребление: 1\nЭффект отопления: 3";
                                break;
                            case 2:
                                ParameterInfos[i].text = $"Режим обогревателя: Древесина";
                                ParameterTips[i]._Info = $"Обогреватель согревает здание за счёт сжигания энергоносителей.\n\nПотребление: 5\nЭффект отопления: 2";
                                break;
                        }
                    }
                    break;
            }
        }

        Describtion.text = Facility._ConstructInfo.Description.Substring(Facility._ConstructInfo.Description.IndexOf("<i>") + 3, Facility._ConstructInfo.Description.IndexOf("</i>") - 3);

        PersonalLabel.text = $"{Facility._AssignedBears.Length}/{Facility._MaxBearCount}";

        float y = 79;
        if (Facility._AssignedBears.Length < Facility._MaxBearCount)
        {
            AddButton.gameObject.SetActive(true);
            AddButton.anchoredPosition = new Vector2(0, -y);
            y += 143;
        }
        else
        {
            AddButton.gameObject.SetActive(false);
        }

        if (Facility is Assimilator && (Facility as Assimilator)._UseRobots)
        {
            RobotButton.gameObject.SetActive(true);
            RobotButton.anchoredPosition = new Vector2(0, -y);
            y += 143;
        }
        else
        {
            RobotButton.gameObject.SetActive(false);
        }

        if (Facility is CyberPaseka)
        {
            CyberBeeButton.gameObject.SetActive(true);
            CyberBeeButton.anchoredPosition = new Vector2(0, -y);
            y += 143;
        }
        else
        {
            CyberBeeButton.gameObject.SetActive(false);
        }

        int count = Facility._AssignedBears.Length;
        if (BearSlots.Length < count)
        {
            BearSlot[] newSlots = new BearSlot[count - BearSlots.Length];
            for (int i = 0; i < count - BearSlots.Length; i++)
            {
                BearSlot slot = Instantiate(BearSlotPrefab, PersonalContent).GetComponent<BearSlot>();
                slot._RectTransform.anchorMin = new Vector2(0.5f, 1);
                slot._RectTransform.anchorMax = new Vector2(0.5f, 1);
                newSlots[i] = slot;
            }

            BearSlots = StaticTools.ExpandMassive(BearSlots, newSlots);
        }
        else if (BearSlots.Length > count)
        {
            while (BearSlots.Length > count)
            {
                Destroy(BearSlots[BearSlots.Length - 1].gameObject);
                BearSlots = StaticTools.ReduceMassive(BearSlots, BearSlots.Length - 1);
            }
        }

        for(int i = 0; i < Facility._AssignedBears.Length; i++)
        {
            BearSlots[i]._RectTransform.anchoredPosition = new Vector2(0, -y);
            BearSlots[i].SetInfo(Facility._AssignedBears[i], i, Click);

            y += 143;
        }
        PersonalContent.sizeDelta = new Vector2(0, y + 15);
    }

    public void Click(int index)
    {
        if(Facility._MaxBearCount == 0)
        {
            return;
        }

        SelectedBear = Facility._AssignedBears[index];

        UserInteract.AskVariants(SelectedBear._Name, new string[] { "Информация", "Снять его с назначения"}, new int[] { 0, 1 }, RightMouseBear);
    }
    public void RightMouseBear(int index)
    {
        switch (index)
        {
            case 0:
                WindowCreator.CreateWindow<BearWindow>().SetInfo(SelectedBear);
                break;
            case 1:
                Facility.AssignBear(SelectedBear, true);
                break;
        }
    }
    public void AddBear()
    {
        int[] exlude = new int[Facility._AssignedBears.Length];
        for(int i = 0; i < exlude.Length; i++)
        {
            exlude[i] = StaticTools.IndexOf(City._DataBase._Bears, Facility._AssignedBears[i]);
        }

        if(Facility is Home)
        {
            UserBear userBear = UserInteract.AskBear("назначить медведя", AddBear, UserBear.Sorting.Facility, "Жилище", UserBear.Sorting.Facility.GetHashCode(), exlude);
            userBear._NoWorkDoHome = true;
        }
        else
        {
            UserBear userBear = UserInteract.AskBear("назначить медведя", AddBear, UserBear.Sorting.Kasta, Facility._RequiredKasta.ToString(), UserBear.Sorting.Facility.GetHashCode(), exlude);
        }
    }
    public void AddBear(Bear bear)
    {
        Facility.AssignBear(bear, false);
    }
    public void AddRobot(int direction)
    {
        Assimilator producer = Facility as Assimilator;
        if (producer != null)
        {
            if(direction > 0)
            {
                if (producer._MaxRobots > producer._Robots && City._Storage._Robots > 0)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{Facility._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Facility)}", (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.Robots));

                    City._Storage._Robots--;
                    producer._Robots++;
                }
            }
            else if (direction < 0)
            {
                if (producer._Robots > 0)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{Facility._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Facility)}", (int)(City._Time._WorldTime / 60), 1, CityStorage.ResourceType.Robots));

                    City._Storage._Robots++;
                    producer._Robots--;
                }
            }
        }
    }
    public void AddBee(int direction)
    {
        CyberPaseka paseka = Facility as CyberPaseka;
        if (paseka != null)
        {
            if (direction > 0)
            {
                if (paseka._MaxCyberBees > paseka._CyberBees && City._Storage._CyberBee > 0)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{Facility._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Facility)}", (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.CyberBee));

                    City._Storage._CyberBee--;
                    paseka._CyberBees++;
                }
            }
            else if (direction < 0)
            {
                if (paseka._CyberBees > 0)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"{Facility._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, Facility)}", (int)(City._Time._WorldTime / 60), 1, CityStorage.ResourceType.CyberBee));

                    City._Storage._CyberBee++;
                    paseka._CyberBees--;
                }
            }
        }
    }

    public void FlyDurandal()
    {
        (Facility as Durandal).RightMouseActions(2);
    }

    public void ChangeUseRobots()
    {
        (Facility as Assimilator).RightMouseActions(5);
    }

    public void ChangeconsumeLimit()
    {
        (Facility as EnergyProcuder).RightMouseActions(5);
    }

    public void ChangeHeatLevel()
    {
        (Facility as OutHeater).RightMouseActions(5);
    }

    public void ChangeProduce()
    {
        if(Facility is Factory)
        {
            Facility.RightMouseActions(5);
        }
    }

    public void Demolish()
    {
        Facility.RightMouseActions(0);
    }

    public void Unassign()
    {
        Facility.Unassign();
    }

    public void AutoAssign()
    {
        Facility.AutoAssign();
    }

    public void ChangeHeaterWork()
    {
        if (Facility == null)
        {
            return;
        }

        UserInteract.AskVariants("Режим обогревателя", new string[] { "Энергомёд", "Древесина", "Выключен" }, new int[] { 1, 2, 0 }, ChangeHeaterWork);
    }
    public void ChangeHeaterWork(int index)
    {
        Facility._Heater = index;
        SoundEffector.PlayEnableDisable(Facility._Heater > 0);
    }

    public void ChangeResearch()
    {
        if (Facility == null)
        {
            return;
        }

        UserInteract.AskVariants("Направление", new string[] { "Электроэнергия", "Отопительные системы", "Медицина и препараты", "Путешествия и логистика", "Жилищные условия", "Пасеки и мёд", "Производство", "Добыча" }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, ChangeResearch);
    }
    public void ChangeResearch(int index)
    {
        (Facility as Laboratory)._Target = (ResearchType)index;
    }

    public override void RightMouse()
    {
        UserInteract.AskVariants(_Label, new string[] { $"{(Pinned ? "открепить" : "закрепить")} окно", "закрыть окно", "Обновить информацию", "Выбрать здание" }, new int[] { -1, 0, 1, 2 }, RightMouseActions);
    }
    public override void RightMouseActions(int index)
    {
        switch (index)
        {
            case -1:
                Pinned = !Pinned;
                break;
            case 0:
                Close();
                break;
            case 1:
                UpdateInfo();
                break;
            case 2:
                if (Facility != null)
                {
                    Facility.Interact();
                }
                break;
        }
    }
}
