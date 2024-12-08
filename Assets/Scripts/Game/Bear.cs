using System;
using UnityEngine;
using static CityFactors;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class Bear
{
    public enum Kasta { Неопределёно, Пасечник, Конструктор, Программист, Биоинженер, Первопроходец, Творец}

    [SerializeField] private Kasta Kast; //Каста
    [SerializeField] private string Name; //Имя, которое медведь получает самостоятельно, но можно будет им дать кличку
    [SerializeField] private int Skill; //Навык, чем выше тем он лучше работает, накапливается со временем работы или учёбы
    [SerializeField] private int Health; //Здоровье, насколько меньше от 10, настолько плохо работает, восстанавливается в больнице. Медведь не может погибнуть 12+ как-никак
    [SerializeField] private int Age; //Возраст, до 6 недееспособен, после может пойти учиться
    [SerializeField] private int BaseHappines; //Базовое значение счастья 
    [SerializeField] private bool Woman; //Пол

    [SerializeField] private Home Home; //Жилье
    [SerializeField] private Facility Facility; //Работа, учёба и тд

    public event SimpleVoid OnSmthChange = null;

    public Kasta _Kasta
    {
        get
        {
            return Kast;
        }
        set
        {
            Kast = value;

            if (OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }
    public string _Name => Name;
    public int _Skill => GetSkill(City._Foodstream._Saturation);
    public int _Health
    {
        get
        {
            return Health;
        }
        set
        {
            Health = Mathf.Clamp(value, 1, 10);

            if (OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }
    public int _Satisfaction => GetSatisfaction(Health, City._Foodstream._Saturation);
    public int _Age => Age;
    public bool _Woman => Woman;
    public int _ColdEndurance
    {
        get
        {
            return (Home != null ? Home._ColdEndurance : 0) + (Facility != null ? Facility._ColdEndurance : ((Home != null ? Home._ColdEndurance : 0))) / 2;
        }
    }
    public string _SaveInfo
    {
        get
        {
            return $"{Kast.GetHashCode()}_{Skill}_{Health}_{BaseHappines}_{Age}_{Woman.GetHashCode()}_{Name}";
        }
        set
        {
            string[] tokents = value.Split('_');

            Kast = (Kasta)int.Parse(tokents[0]);
            Skill = int.Parse(tokents[1]);
            Health = int.Parse(tokents[2]);
            BaseHappines = int.Parse(tokents[3]);
            Age = int.Parse(tokents[4]);
            Woman = tokents[5] == "1";
            Name = tokents[6];
        }
    }
    public Home _Home
    {
        get
        {
            return Home;
        }
        set
        {
            Home = value;

            if (OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }
    public Facility _Facility
    {
        get
        {
            return Facility;
        }
        set
        {
            Facility = value;

            if (OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }

    public void SkillUp(int value)
    {
        Skill = Mathf.Clamp(Skill + value, 1, 999);

        if (OnSmthChange != null)
        {
            OnSmthChange.Invoke();
        }
    }
    public int GetSatisfaction(int health, float saturation)
    {
        float value = BaseHappines;

        foreach (Factor factor in City._Factors._SatisfactionFactors)
        {
            value += factor.Value;
        }

        if (Home == null)
        {
            value -= 3;
        }
        else
        {
            if (Home._Effectivity < 0.75f)
            {
                value -= 1;
            }
            else if (Home._Effectivity >= 1 && Home._SatisfactionBonus > 0)
            {
                value += Home._SatisfactionBonus;
            }
        }
        if (Facility == null)
        {
            value -= 1;
        }
        else
        {
            if (Facility._RequiredKasta != Kast)
            {
                value -= 2;
            }
        }
        if ((Home != null && Home._AssignedBears.Length == 1) || (Facility != null && Facility._AssignedBears.Length == 1))
        {
            value -= 1;
        }
        if (health > 1 && health < 4)
        {
            value -= 2;
        }
        else if (health < 5)
        {
            value -= 1.5f;
        }
        if (saturation >= 1)
        {
            value += 2;
        }
        else if (saturation < 0.25f)
        {
            value -= 3;
        }
        else if (saturation < 0.75f)
        {
            value -= 1;
        }
        if(_ColdEndurance - City._Weather._Cold > 0)
        {
            value += 2;
        }
        else if(_ColdEndurance - City._Weather._Cold < -3)
        {
            value -= 3;
        }
        if (City._Buyiments._EmotionBuffer)
        {
            value += 3;
        }

        return Mathf.Clamp(Mathf.RoundToInt(value), 0, 10);
    }
    public int GetSkill(float saturation)
    {
        float value = Skill;

        value *= Mathf.Clamp(saturation, 0.5f, 1.25f);
        value *= Health / 8f;

        if (Facility != null)
        {
            if (Facility._RequiredKasta != Kast)
            {
                value *= 0.25f;
            }
        }

        return (int)value;
    }
    public CityFactors.Factor[] GetSatisfactionFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[City._Factors ._SatisfactionFactors.Length + 1];
        factors[0] = new Factor("Базовое значение", BaseHappines, -1);
        System.Array.Copy(City._Factors._SatisfactionFactors,0, factors, 1, factors.Length - 1);

        if(Home == null)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Отсутствие жилья", -3, -1));
        }
        else
        {
            if(Home._Effectivity < 0.75f)
            {
                factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Перебои электричества", -1, -1));
            }
            else if(Home._Effectivity >= 1 && Home._SatisfactionBonus > 0)
            {
                factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Комфорт и уют дома", Home._SatisfactionBonus, -1));
            }
        }
        if(Facility == null)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Не занят", -1, -1));
        }
        else
        {
            if(Facility._RequiredKasta != Kast)
            {
                factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Работа не по специальности", -2, -1));
            }
        }
        if ((Home != null && Home._AssignedBears.Length == 1) || (Facility != null && Facility._AssignedBears.Length == 1))
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Одинок", -1, -1));
        }
        if (Health > 1 && Health < 4)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Предкриогенное состояние", -2, -1));
        }
        else if(Health < 5)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Не здоров", -1.5f, -1));
        }
        if(City._Foodstream._Saturation >= 1)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Полностью сыт", 2, -1));
        }
        else if (City._Foodstream._Saturation < 0.25f)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Голод", -3, -1));
        }
        else if(City._Foodstream._Saturation < 0.75f)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Недоедает", -1, -1));
        }
        if (_ColdEndurance - City._Weather._Cold > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Тепло", 2, -1));
        }
        else if (_ColdEndurance - City._Weather._Cold < -3)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Очень холодно", -3, -1));
        }
        if (City._Buyiments._EmotionBuffer)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor("Стабилизатор эмоций", 3, -1));
        }

        return factors;
    }
    public CityFactors.Factor[] GetSkillFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[3];

        factors[0] = new CityFactors.Factor("Базовое значение", Skill, -1);
        factors[1] = new CityFactors.Factor("Коэффициент сытости", Mathf.Clamp(City._Foodstream._Saturation, 0.5f, 1.25f), -1, true);
        factors[2] = new CityFactors.Factor("Коэффициент здоровья", Health / 8f, -1, true);

        if (Facility != null)
        {
            if (Facility._RequiredKasta != Kast)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Работа не по специальности", 0.25f, -1, true));
            }
        }

        return factors;
    }

    public Bear() 
    {

    }

    public Bear(Kasta kast, string name, int skill, int health, int baseHappines, int age, bool woman)
    {
        Kast = kast;
        Name = name;
        Skill = skill;
        Health = health;
        BaseHappines = baseHappines;
        Age = age;
        Woman = woman;
    }
}
