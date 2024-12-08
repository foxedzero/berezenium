
using UnityEngine;

public class CityResearch : MonoBehaviour
{
    public enum ResearchType {Electricity, Cold, Medicine, Travels, Household, Food, Production, Mining}

    [SerializeField] private float Electricity;
    [SerializeField] private float Cold;
    [SerializeField] private float Medicine;
    [SerializeField] private float Travels;
    [SerializeField] private float Household;
    [SerializeField] private float Food;
    [SerializeField] private float Production;
    [SerializeField] private float Mining;

    private Laboratory[] Laboratories = new Laboratory[0];

    public event SimpleVoid OnReseatchUpdate = null;

    public string _SaveInfo
    {
        get
        {
            return $"{Electricity};{Cold};{Medicine};{Travels};{Household};{Food};{Production};{Mining}";
        }
        set
        {
            if (value == null || value.Length <=0)
            {
                return;
            }

            string[] tokens = value.Split(";");

            Electricity = StaticTools.StringToFloat(tokens[0]);
            Cold = StaticTools.StringToFloat(tokens[1]);
            Medicine = StaticTools.StringToFloat(tokens[2]);
            Travels = StaticTools.StringToFloat(tokens[3]);
            Household = StaticTools.StringToFloat(tokens[4]);
            Food = StaticTools.StringToFloat(tokens[5]);
            Production = StaticTools.StringToFloat(tokens[6]);
            Mining = StaticTools.StringToFloat(tokens[7]);
        }
    }

    private void Start()
    {
        City._DataBase.OnFacilityChanges += UpdateLaboratories;
    }

    public void UpdateLaboratories()
    {
        Laboratories = new Laboratory[0];

        foreach(Facility facility in City._DataBase._Facilities)
        {
            Laboratory laboratory = facility as Laboratory;
            if(laboratory != null)
            {
                Laboratories = StaticTools.ExpandMassive(Laboratories, laboratory);
                laboratory.SetRequiredSkill(GetResearchGoal(laboratory._Target));
            }
        }
    }
    public void DayPassed()
    {
        foreach(Laboratory laboratory in Laboratories)
        {
            Research(laboratory._Target, laboratory._ResearchPoints);
            laboratory.SetRequiredSkill(GetResearchGoal(laboratory._Target));
        }

        if (OnReseatchUpdate != null)
        {
            OnReseatchUpdate.Invoke();
        }
    }

    public int GetResearchLevel(ResearchType type)
    {
        float score = GetResearch(type);
        if (score >= 4000)
        {
            return 3;
        }
        else if (score >= 2000)
        {
            return 2;
        }
        else if (score >= 1000)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public float GetResearch(ResearchType type)
    {
        switch (type)
        {
            case ResearchType.Electricity:
                return Electricity;
            case ResearchType.Cold:
                return Cold;
            case ResearchType.Medicine:
                return Medicine;
            case ResearchType.Travels:
                return Travels;
            case ResearchType.Household:
                return Household;
            case ResearchType.Food:
                return Food;
            case ResearchType.Production:
                return Production;
            case ResearchType.Mining:
                return Mining;
        }

        return 0;
    }
    public float GetResearchGoal(ResearchType type)
    {
        switch (type)
        {
            case ResearchType.Electricity:
                return GetGoal(Electricity);
            case ResearchType.Cold:
                return GetGoal(Cold);
            case ResearchType.Medicine:
                return GetGoal(Medicine);
            case ResearchType.Travels:
                return GetGoal(Travels);
            case ResearchType.Household:
                return GetGoal(Household);
            case ResearchType.Food:
                return GetGoal(Food);
            case ResearchType.Production:
                return GetGoal(Production);
            case ResearchType.Mining:
                return GetGoal(Mining);
        }

        return -1;
    }

    private void Research(ResearchType type, float value)
    {
        int level = GetResearchLevel(type);

        switch (type)
        {
            case ResearchType.Electricity:
                Electricity += value;
                break; 
            case ResearchType.Cold:
                Cold += value;
                break;
            case ResearchType.Medicine:
                Medicine += value;
                break;
            case ResearchType.Travels:
                Travels += value;
                break;
            case ResearchType.Household:
                Household += value;
                break;
            case ResearchType.Food:
                Food += value;
                break;
            case ResearchType.Production:
                Production += value;
                break;
            case ResearchType.Mining:
                Mining += value;
                break;
        }

        int newLv = GetResearchLevel(type);
        if (level != newLv)
        {
            CityMessenger.CityMessage message = new CityMessenger.CityMessage("Прорыв в исследовании", "", false);
            message.Fest = true;
            switch (type)
            {
                case ResearchType.Electricity:
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Уровень исследования электроэнергии повысился. Теперь вы можете строить аккумуляторы, которые позволят вам хранить избыточное электричество.";
                            break;
                        case 2:
                            message.Info = $"Уровень исследования электроэнергии повысился. Эффективность электростанции увелина на 25%.";
                            break;
                        case 3:
                            message.Info = $"Уровень исследования электроэнергии повысился. Была разработана технология получение энергии антигравитации, вы можете построить березениумную электростанцию.";
                            break;
                    }
                    break;
                case ResearchType.Cold:
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Уровень исследования отопления повысился. Теперь в зданиях установлены обогреватели. Вы можете их включить в окне здания, они будут давать 2 ед. к хладостойкости, но при этом потреблять ресурс.";
                            break;
                        case 2:
                            message.Info = $"Уровень исследования отопления повысился.";
                            break;
                        case 3:
                            message.Info = $"Уровень исследования отопления повысился.";
                            break;
                    }
                    break;
                case ResearchType.Medicine:
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Уровень исследования медицины повысился.";
                            break;
                        case 2:
                            message.Info = $"Уровень исследования медицины повысился. Отныне медпункты лечат на 1 здоровье больше, а также берут на 3 пациеента больше.";
                            break;
                        case 3:
                            message.Info = $"Уровень исследования медицины повысился.";
                            break;
                    }
                    break;
                case ResearchType.Travels:
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Уровень исследования путешествий повысился. Медведи, которые ведут разведки, теперь защищены от холода на 3 единицы больше.";
                            break;
                        case 2:
                            message.Info = $"Уровень исследования путешествий повысился. С помощью снегоходов медведи будут быстрее (на 50%) и успешнее вести поиски.";
                            break;
                        case 3:
                            message.Info = $"Уровень исследования путешествий повысился. Вы можете построить космолёт \"Дюрандаль\".";
                            break;
                    }
                    break;
                case ResearchType.Household:
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Уровень исследования жилища повысился. Вы можете построить новый дом - многоэтажка. Она защищенней от холода, а также более вместительна, но дороже в стоимости.";
                            break;
                        case 2:
                            message.Info = $"Уровень исследования жилища повысился. Была увеличена хладостойкость домов на 3 единицы.";
                            break;
                        case 3:
                            message.Info = $"Уровень исследования жилища повысился.";
                            break;
                    }
                    break;
                case ResearchType.Food:
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Уровень исследования пищи повысился. Хладостойкость пасек увеличена на 3 единицы.";
                            break;
                        case 2:
                            message.Info = $"Уровень исследования пищи повысился. Пасеки теперь приносят на 10 мёда больше.";
                            break;
                        case 3:
                            message.Info = $"Уровень исследования пищи повысился.";
                            break;
                    }
                    break;
                case ResearchType.Production:
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Уровень исследования производства повысился. Теперь вы можете построить химзавод энергомёда.";
                            break;
                        case 2:
                            message.Info = $"Уровень исследования производства повысился. Производство потребляет на 25% меньше ресурсов.";
                            break;
                        case 3:
                            message.Info = $"Уровень исследования производства повысился.";
                            break;
                    }
                    break;
                case ResearchType.Mining:
                    switch (newLv)
                    {
                        case 1:
                            message.Info = $"Уровень исследования добычи повысился. Хладостойкость добывающих точек повышена на 3 ед..";
                            break;
                        case 2:
                            message.Info = $"Уровень исследования добычи повысился. Эффективность добычи увеличена на 25%.";
                            break;
                        case 3:
                            message.Info = $"Уровень исследования добычи повысился.";
                            break;
                    }
                    break;
            }

            City._CityMessenger.SetMessage(message, false);
        }
    }

    private float GetGoal(float score)
    {
        if (score >= 4000)
        {
            return -1;
        }
        else if (score >= 2000)
        {
            return 4000;
        }
        else if (score >= 1000)
        {
            return 2000;
        }
        else
        {
            return 1000;
        }
    }

    public void LabolatoryRetarget(Laboratory laboratory)
    {
        laboratory.SetRequiredSkill(GetResearchGoal(laboratory._Target));

        if (OnReseatchUpdate != null)
        {
            OnReseatchUpdate.Invoke();
        }
    }
}
