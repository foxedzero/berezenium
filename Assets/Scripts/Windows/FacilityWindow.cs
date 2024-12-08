
using UnityEngine;
using UnityEngine.UI;
using static CityResearch;

public class FacilityWindow : DefaultWindow
{
    [SerializeField] private GameObject ParameterPrefab;
    [SerializeField] private GameObject BearHolderPrefab;
    [SerializeField] private Image Preview;
    [SerializeField] private RectTransform RobotButton;
    [SerializeField] private RectTransform ResearchButton;
    [SerializeField] private RectTransform EnabledButton;
    [SerializeField] private RectTransform HeaterButton;

    [SerializeField] private Text Name;
    [SerializeField] private Text Category;
    [SerializeField] private Text Address;
    [SerializeField] private Text Coordinates;
    [SerializeField] private Text RobotsInfo;
    [SerializeField] private Text ResearchInfo;
    [SerializeField] private Text EnabledInfo;
    [SerializeField] private Text HeaterInfo;

    [SerializeField] private Text[] ParameterInfos;
    [SerializeField] private Tipper[] ParameterTips;

    [SerializeField] private Text Describtion;

    [SerializeField] private IlusionHolders IlusionHolders;
    [SerializeField] private Text Holders;
    [SerializeField] private RectTransform HolderContent;

    [SerializeField] private RectTransform Content;
    [SerializeField] private Text AssignedBearsLabel;
    [SerializeField] private Facility Facility;

    private Bear SelectedBear = null;
    private int[] Configuration = new int[0];
    //0 - Спец.   1 - Навык   2 - Электропотр.   3 - Эффективность
    //4 - вместительность жилья   5 - бонус жилья   6 - хладоустойчивость
    //7 - коминал электричества
    //8 - электроёмкость
    //9 - номинал еды
    //10 - хранение еды
    //11 - объем работы строительства
    //12 - номинал ресов  13 - осталось ресов 17 - тип ресурсов
    //14 - количество роботов
    //15 - номинал роботов
    //16 - потребление ресов
    //18 - уровень лечения 19 - количество пациентов
    //20 - прогресс разведки
    //21 - очки исследования

    public override string _Label => $"окно информации о \"{Facility._ConstructInfo.Name}\""; 

    public Facility _Facility => Facility;

    private void OnDestroy()
    {
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

        Facility.OnSmthChange += UpdateInfo;
        Facility.OnFacilityDestroy += Close;

        IlusionHolders.SetInfo(null, Click);

        Configuration = new int[0] {};
        if(facility is Home)
        {
            Configuration = new int[] {2, 4, 5, 6};
        }
        else if(facility is EnergyProcuder)
        {
            Configuration = new int[] {0, 1, 3, 7, 16};
        }
        else if(facility is Accumulator)
        {
            Configuration = new int[] {8};
        }
        else if (facility is FoodProducer)
        {
            Configuration = new int[] { 0, 1, 2, 3, 6, 9};
        }
        else if(facility is FoodStorage)
        {
            Configuration = new int[] {10};
        }
        else if(facility is ConstructionProject)
        {
            Configuration = new int[] { 0, 1, 2, 3, 6, 11};
        }
        else if (facility is ExsaustProducer)
        {
            Configuration = new int[] { 0, 1, 2, 3, 6, 17, 12, 13, 14 };
        }
        else if(facility is Producer)
        {
            Configuration = new int[] {0, 1, 2, 3, 6, 17, 12, 14, 16};
        }
        else if (facility is RobotFactory)
        {
            Configuration = new int[] { 0, 1, 2, 3, 6, 15, 16 };
        }
        else if (facility is Laboratory)
        {
            Configuration = new int[] { 0, 1, 2, 3, 6 , 21};
        }
        else if (facility is MedFacility)
        {
            Configuration = new int[] { 0, 1, 2, 3, 6, 18, 19};
        }
        else if (facility is Finders)
        {
            Configuration = new int[] { 0, 1, 2, 3, 6, 20};
        }

        foreach (Text info in ParameterInfos)
        {
            Destroy(info.gameObject);
        }

        RobotButton.gameObject.SetActive(false);
        ParameterInfos = new Text[Configuration.Length];
        ParameterTips = new Tipper[Configuration.Length];
        for (int i = 0; i < Configuration.Length; i++)
        {
            GameObject paramter = Instantiate(ParameterPrefab, Content);
            ParameterInfos[i] = paramter.GetComponent<Text>();
            ParameterTips[i] = paramter.GetComponent<Tipper>();

            paramter.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -220 - 30 * i);

            if (Configuration[i] == 14)
            {
                RobotButton.gameObject.SetActive(true);
            }
        }

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        if(Facility == null)
        {
            Destroy(gameObject);
            return;
        }


        if(Facility is ConstructionProject)
        {
            Preview.sprite = (Facility as ConstructionProject)._Construction.Icon;
            Name.text = $"проект \"{(Facility as ConstructionProject)._Construction.Name}\"";
            Category.text = $"Тип: Проект постройки";
        }
        else
        {
            Preview.sprite = Facility._ConstructInfo.Icon;
            Name.text = Facility._ConstructInfo.Name;
            Category.text = $"Тип: {Facility._ConstructInfo.Category}";
        }

        Address.text = $"Номер: #{StaticTools.IndexOf(City._DataBase._Facilities, Facility)}";
        Coordinates.text = $"Координаты: x{Facility.transform.position.x} z{Facility.transform.position.z}";

        for(int i = 0; i < Configuration.Length; i++)
        {
            switch (Configuration[i])
            {
                case 0:
                    ParameterInfos[i].text = $"Специальность: {Facility._RequiredKasta}";
                    ParameterTips[i]._Info = $"Специализация медведя, который будет назначен здесь работать\r\n\r\nМедведь с иной специализацией работает в 4 раза хуже";
                    break;
                case 1:
                    {
                        float summ = 0;
                        foreach (Bear bear1 in Facility._AssignedBears)
                        {
                            summ += bear1._Skill;
                        }
                        ParameterInfos[i].text = $"Опыт: {summ}/{Facility._RequiredSkill}";

                        string info = "Требуемый суммарный опыт всех рабочих для нормальной работы здания.\n\nМедведи:";
                        foreach (Bear bear1 in Facility._AssignedBears)
                        {
                            info += $"\n{bear1._Name}  {bear1._Skill}";
                        }

                        ParameterTips[i]._Info = info;
                    }
                    break;
                case 2:
                    ParameterInfos[i].text = $"Потребление электричества: {Facility._EnergyConsume}";
                    ParameterTips[i]._Info = $"Электричество, которе требуется для нормальной работы здания.\n\nМинимальный коэффициент от электричества: {Facility._MinimalEnergyCoefficiente}";
                    break;
                case 3:
                    {
                        ParameterInfos[i].text = $"Эффективность: {(int)(Facility._Effectivity * 100)}%";

                        string info = $"Эффективность здания, определяющая результат работы.\n\nФакторы:";
                        foreach (CityFactors.Factor factor in Facility.GetEffectivityFactors())
                        {
                            info += $"\n{factor}";
                        }

                        ParameterTips[i]._Info = info;
                    }
                    break;
                case 4:
                    ParameterInfos[i].text = $"Вместительность жилья: {(Facility as Home)._MaxBearCount}";
                    ParameterTips[i]._Info = $"Вместительность жилья определяет максимальное количество медведей, которых можно заселить.";
                    break;
                case 5:
                    ParameterInfos[i].text = $"Бонус жилья: {(Facility as Home)._SatisfactionBonus}";
                    ParameterTips[i]._Info = $"Бонус к удовлетворенности медведя, проживающего в данном доме.";
                    break;
                case 6:
                    ParameterInfos[i].text = $"Хладостойкость: {Facility._ColdEndurance}";
                    ParameterTips[i]._Info = $"Хладостойкость влияет на терпимость к внешнему холоду.\nЧем холод больше стойкости, тем выше риск заболеть медведю и выше его недовольство.";
                    break;
                case 7:
                    ParameterInfos[i].text = $"Базовая электровыработки: {(Facility as EnergyProcuder)._BaseProduce}";
                    ParameterTips[i]._Info = $"Выработка электричества при нормальных условиях.\n\nТекущая потенциальная выработка: {(Facility as EnergyProcuder).GetPotencialEffectivity() * (Facility as EnergyProcuder)._BaseProduce}";
                    break;
                case 8:
                    ParameterInfos[i].text = $"Электроёмкость: {(Facility as Accumulator)._Capacity}";
                    ParameterTips[i]._Info = $"Количество электричества, которое сможет сохранить аккумулятор.";
                    break;
                case 9:
                    ParameterInfos[i].text = $"Базовая выработка: {(Facility as FoodProducer)._BaseProduce}";
                    ParameterTips[i]._Info = $"Сколько еды принесет здание при нормальных условиях.\n\nТекущая потенциальая выработка: {(Facility as FoodProducer).GetPotencialEffectivity() * (Facility as FoodProducer)._BaseProduce}";
                    break;
                case 10:
                    ParameterInfos[i].text = $"Вместимость еды: {(Facility as FoodStorage)._Capacity}";
                    ParameterTips[i]._Info = $"Количество еды, которое может храниться на складе.";
                    break;
                case 11:
                    ParameterInfos[i].text = $"Объем работы строительства: {(Facility as ConstructionProject)._WorkLeft}";
                    ParameterTips[i]._Info = $"Количество дней до конца стройки. При нормальных условиях снижается на 1 за день.\n\nТекущее потенциальное снижение за день: {(Facility as ConstructionProject).GetPotencialEffectivity()}";
                    break;
                case 12:
                    ParameterInfos[i].text = $"Номинал ресурсов: {(Facility as Producer)._Nominal}";
                    ParameterTips[i]._Info = $"Количество ресурсов, получаемых при нормальных условиях.\n\nТекущее потенциальное получение: {Facility.GetPotencialEffectivity() * (Facility as Producer)._Nominal}";
                    break;
                case 13:
                    ParameterInfos[i].text = $"Осталось ресурсов: {(Facility as ExsaustProducer)._ResourcesLeft}";
                    ParameterTips[i]._Info = $"Количество оставшихся ресурсов. Если они закончатся, то этим местом уже нельзя будет воспользоваться.";
                    break;
                case 14:
                    RobotsInfo.text = $"Назначено роботов: {(Facility as Producer)._Robots}/{(Facility as Producer)._RequiredRobots}";
                    break;
                case 15:
                    ParameterInfos[i].text = $"Производство роботов: {(Facility as RobotFactory)._BaseProduce}";
                    ParameterTips[i]._Info = $"Сколько будет построено роботов за день при нормальных условиях. Число приведется к целому с округлением вниз.\n\nТекущее потенциальное производство: {(Mathf.FloorToInt((Facility as RobotFactory)._BaseProduce * Facility.GetPotencialEffectivity()))}";
                    break;
                case 17:
                    ParameterInfos[i].text = $"Тип ресурса: {CityStorage.ResourceName( (Facility as Producer)._ResourceType)}";
                    ParameterTips[i]._Info = $"Количество роботов для нормальной работы.";
                    break;
                case 16:
                    if(Facility is Producer)
                    {
                        if((Facility as Producer)._ConsumeResource == CityStorage.ResourceType.None)
                        {
                            ParameterTips[i]._Info = $"Здание не требует никаких ресурсов для производства.";
                            ParameterInfos[i].text = $"Не требует ресурсов";
                        }
                        else
                        {
                            ParameterInfos[i].text = $"Потребление {CityStorage.ResourceName((Facility as Producer)._ConsumeResource)}: {(Facility as Producer)._Consume}";
                            ParameterTips[i]._Info = $"Треубемое количество ресурса для работы.";
                        }
                    }
                    else if(Facility is RobotFactory)
                    {
                        ParameterInfos[i].text = $"Потребление металла: {(Facility as RobotFactory)._RobotCost}";
                        ParameterTips[i]._Info = $"Треубемое количество ресурса для производства одного робота.";
                    }
                    else if (Facility is EnergyProcuder)
                    {
                        ParameterInfos[i].text = $"Потребление {CityStorage.ResourceName((Facility as EnergyProcuder)._Resource)}: {(Facility as EnergyProcuder)._BaseConsume}";
                        ParameterTips[i]._Info = $"Треубемое количество ресурса для максимальной выработки электричества.";
                    }
                    break;
                case 18:
                    ParameterInfos[i].text = $"Лечение: {(Facility as MedFacility)._Healing} ед.";
                    ParameterTips[i]._Info = $"Количество излечиваемоего здоровья пациента-медведя.";
                    break;
                case 19:
                    ParameterInfos[i].text = $"Количество пациентов: {(Facility as MedFacility)._HealCount}.";
                    ParameterTips[i]._Info = $"Сколько медведей будет вылечино за день.";
                    break;
                case 20:
                    ParameterInfos[i].text = $"Оставшее время разведки: {(Facility as Finders)._Progress}.";
                    ParameterTips[i]._Info = $"Сколько дней осталось до спасения еще одного медведя.\n\nПотенциальное изменение {-Facility.GetPotencialEffectivity()} дней";
                    break;
                case 21:
                    ParameterInfos[i].text = $"Номинальное количество очков изучения: {(Facility as Laboratory)._BaseResearchPoints}.";
                    ParameterTips[i]._Info = $"Очки, которые пойдут на выбранное исследование.\n\nПотенциальное изучение: {Facility.GetPotencialEffectivity() * (Facility as Laboratory)._BaseResearchPoints} очков";
                    break;
            }
        }

        Describtion.text = Facility._ConstructInfo.Description.Substring(Facility._ConstructInfo.Description.IndexOf("<i>") + 3, Facility._ConstructInfo.Description.IndexOf("</i>") - 3);
        float prefHeight = Describtion.cachedTextGenerator.GetPreferredHeight(Describtion.text, Describtion.GetGenerationSettings(new Vector2(RectTransform.sizeDelta.x, 0)));
        Describtion.rectTransform.sizeDelta = new Vector2(0, prefHeight);
        float y = 220 + Configuration.Length * 30 + prefHeight;

        Describtion.rectTransform.anchoredPosition = new Vector2(0, - y + prefHeight - 30);

        y += 27.5f;

        if (Facility._CanBeDisabled)
        {
            y += 30;
            EnabledButton.gameObject.SetActive(true);
            EnabledButton.anchoredPosition = new Vector2(0, -y);
            EnabledInfo.text = Facility._Enabled ? $"Работа не приостановлена" : "Работа приостановлена";
        }
        else
        {
            EnabledButton.gameObject.SetActive(false);
        }

        if (City._Research.GetResearchLevel(CityResearch.ResearchType.Cold) > 0 && Facility._CanBeHeated)
        {
            y += 30;
            HeaterButton.anchoredPosition = new Vector2(0, -y);
            HeaterButton.gameObject.SetActive(true);

            switch (Facility._Heater)
            {
                case 0:
                    HeaterInfo.text = "Обогреватель выключен";
                    break;
                case 1:
                    HeaterInfo.text = "Обогреватель работает на электричестве";
                    break;
                case 2:
                    HeaterInfo.text = "Обогреватель работает на энергомёде";
                    break;
                case 3:
                    HeaterInfo.text = "Обогреватель работает на древесине";
                    break;
            }
        }
        else
        {
            HeaterButton.gameObject.SetActive(false);
        }

        if (StaticTools.Contains(Configuration, 14))
        {
            y += 30;
            RobotButton.gameObject.SetActive(true);
            RobotButton.anchoredPosition = new Vector2(0, -y);
        }
        else
        {
            RobotButton.gameObject.SetActive(false);
        }

        if (Facility is Laboratory)
        {
            y += 30;
            switch((Facility as Laboratory)._Target)
            {
                case CityResearch.ResearchType.Electricity:
                    ResearchInfo.text = $"Исследование: Электроэнергия";
                    break;
                case CityResearch.ResearchType.Cold:
                    ResearchInfo.text = $"Исследование: Отопительные системы";
                    break;
                case CityResearch.ResearchType.Medicine:
                    ResearchInfo.text = $"Исследование: Медицина и препараты";
                    break;
                case CityResearch.ResearchType.Travels:
                    ResearchInfo.text = $"Исследование: Путешествия и логистика";
                    break;
                case CityResearch.ResearchType.Household:
                    ResearchInfo.text = $"Исследование: Жилищные условия";
                    break;
                case CityResearch.ResearchType.Food:
                    ResearchInfo.text = $"Исследование: Пасеки и мёд";
                    break;
                case CityResearch.ResearchType.Production:
                    ResearchInfo.text = $"Исследование: Производство";
                    break;
                case CityResearch.ResearchType.Mining:
                    ResearchInfo.text = $"Исследование: Добыча";
                    break;
            }
            ResearchButton.gameObject.SetActive(true);
            ResearchButton.anchoredPosition = new Vector2(0, -y);
        }
        else
        {
            ResearchButton.gameObject.SetActive(false);
        }

        y += 62.5f;
        AssignedBearsLabel.text = $"{Facility._AssignedBears.Length}/{Facility._MaxBearCount} медведей";
        AssignedBearsLabel.rectTransform.anchoredPosition = new Vector2(0, -y);
        y += 17.5f;

        bool addbear = Facility._AssignedBears.Length < Facility._MaxBearCount;
        string names = addbear ? $"Назначить медведя\n" : "";
        foreach(Bear bear in Facility._AssignedBears)
        {
            names += $"{bear._Name} #{StaticTools.IndexOf(City._DataBase._Bears, bear)}\n";
        }

        HolderContent.anchoredPosition = new Vector2(0, -y);
        Holders.text = names;
        HolderContent.sizeDelta = new Vector2(0, Facility._AssignedBears.Length * 35 + (addbear ? 35 : 0));
        IlusionHolders._MaxIndex = Facility._AssignedBears.Length + 1;

        y += Facility._AssignedBears.Length * 35 + (addbear ? 35 : 0);

        Content.sizeDelta = new Vector2(0, y + 15);
    }

    public void Click(int index)
    {
        if(Facility._MaxBearCount == 0)
        {
            return;
        }

        if(Facility._AssignedBears.Length < Facility._MaxBearCount)
        {
            if(index == 0)
            {
                AddBear();
                return;
            }
            else
            {
                SelectedBear = Facility._AssignedBears[index - 1];
            }
        }
        else
        {
            SelectedBear = Facility._AssignedBears[index];
        }

        UserInteract.AskVariants(SelectedBear._Name, new string[] { "Информация", "Снять его с назначения", "Назначить вместо него" }, new int[] { 0, 1, 2 }, RightMouseBear);
    }
    public void RightMouseBear(int index)
    {
        switch (index)
        {
            case 0:
                FindObjectOfType<WindowCreator>().CreateWindow<BearWindow>().SetInfo(SelectedBear);
                break;
            case 1:
                Facility.AssignBear(SelectedBear, true);
                break;
            case 2:
                UserInteract.AskBear("Назначить вместо него", Resign);
                break;
        }
    }
    public void AddBear()
    {
        UserBear userBear = UserInteract.AskBear("назначить медведя", AddBear);

        if(Facility is Home)
        {
            userBear.SetBearSorting(3);
            userBear.SetBearKasta(-1);
        }
        else
        {
            userBear.SetBearSorting(2);
            userBear.SetBearKasta(Facility._RequiredKasta.GetHashCode());
        }
    }
    public void AddBear(Bear bear)
    {
        Facility.AssignBear(bear, false);
    }
    public void Resign(Bear bear)
    {
        Facility.AssignBear(SelectedBear, true);
        Facility.AssignBear(bear, false);
    }

    public void ClickRobot()
    {
        if (Input.GetMouseButtonUp(1))
        {
            AddRobot(-1);
        }
        else
        {
            AddRobot(1);
        }
    }
    public void AddRobot(int direction)
    {
        Producer producer = Facility as Producer;
        if (producer != null)
        {
            if(direction > 0)
            {
                if (producer._RequiredRobots > producer._Robots && City._Storage._Robots > 0)
                {
                    City._Storage._Robots--;
                    producer._Robots++;
                }
            }
            else if (direction < 0)
            {
                if (producer._Robots > 0)
                {
                    City._Storage._Robots++;
                    producer._Robots--;
                }
            }
        }
    }

    public void ChangeHeaterWork()
    {
        if (Facility == null)
        {
            return;
        }

        UserInteract.AskVariants("Режим обогревателя", new string[] { "Электричество", "Энергомёд", "Древесина", "Выключен" }, new int[] { 1, 2, 3, 0 }, ChangeHeaterWork);
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

    public void ChangeEnable()
    {
        if (Facility == null)
        {
            return;
        }

        if (Facility._Enabled)
        {
            UserInteract.AskConfirm("Приостановка работы", "Если вы приостановите здание, что все медведи будут сняты с позиций, а здание прекратит потреблять и совершать работу.", DisableFacility);
        }
        else
        {
            Facility._Enabled = true;
            SoundEffector.PlayEnableDisable(Facility._Enabled);
        }
    }
    public void DisableFacility(bool state)
    {
        if (state)
        {
            Facility._Enabled = false;
            SoundEffector.PlayEnableDisable(Facility._Enabled);
        }
    }

    public void RightMouseTask()
    {
        if(Facility != null)
        {
            Facility.RightMouse();
        }
    }
    public override void RightMouse()
    {
        UserInteract.AskVariants(_Label, new string[] { $"{(Pinned ? "открепить" : "закрепить")} окно", "закрыть окно", "Обновить информацию", "Дать распоряжение" }, new int[] { -1, 0, 1, 2 }, RightMouseActions);
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
                RightMouseTask();
                break;
        }
    }
}
