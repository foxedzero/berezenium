using System;
using System.Collections.Generic;
using UnityEngine;
using static CityFactors;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class Bear
{
    public enum Kasta { Неопределено, Пасечник, Конструктор, Программист, Биоинженер, Первопроходец, Творец}

    [SerializeField] protected Kasta Kast; //Каста
    [SerializeField] protected string Name; //Имя, которое медведь получает самостоятельно, но можно будет им дать кличку
    [SerializeField] protected int Health; //Здоровье, насколько меньше от 10, настолько плохо работает, восстанавливается в больнице. Медведь не может погибнуть 12+ как-никак
    [SerializeField] protected int Age; //Возраст, до 6 недееспособен, после может пойти учиться

    [SerializeField] protected int Face;
    [SerializeField] protected int Brows;
    [SerializeField] protected int BodyColor;

    [SerializeField] protected Facility CurrentFacility; 
    [SerializeField] protected Home Home; //Жилье
    [SerializeField] protected Facility Facility; //Работа, учёба и тд
    [SerializeField] protected BearObject Object;

    [SerializeField] protected float Tired = 0;
    [SerializeField] protected float Stress = 0;
    [SerializeField] protected int Schedule = 0;
    [SerializeField] protected int[] Effects = new int[5]; // 0 - антистресс, 1 - тоник, 2 - стимулятор, 3 - антиспячкин, 4- радость спасения
    [SerializeField] protected bool AtWork;

    protected CitySally.Sally Sally = null;

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
    public int _Health
    {
        get
        {
            return Health;
        }
        set
        {
            Health = Mathf.Clamp(value, 1, 10);

            if(Object != null)
            {
                Object.CheckHealth();
            }

            if (OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }
    public int _Age => Age;

    public virtual float _TiredCoefficient
    {
        get
        {
            if(Sally != null)
            {
                return 0;
            }

            return 1 * (Stress > 25 ? (1 + Stress / 200) : 1) * (Effects[1] > 0 ? 0.75f : 1) * (Effects[2] > 0 ? 1.5f : 1);
        }
    }
    public float _Tired
    {
        get
        {
            return Tired;
        }
        set
        {
            Tired = Mathf.Clamp01(value);

            OnSmthChange?.Invoke();
        }
    }
    public float _Stress
    {
        get
        {
            if (_Nedeesposoben)
            {
                return 100;
            }

            return Stress;
        }
        set
        {
            Stress = Mathf.Clamp(value, 0, 100);

            OnSmthChange?.Invoke();
        }
    }
    public virtual float _StressDelta
    {
        get
        {
            float value = 0;

            if (Effects[0] > 0)
            {
                value -= 10;
            }

            if (Effects[4] > 0)
            {
                value -= 40;
            }

            if (_HeatLevel < -2)
            {
                value += 6;
            }
            else if (_HeatLevel < 0)
            {
                value += 15;
            }
            else
            {
                value -= 1;
            }

            if (CurrentFacility != null && CurrentFacility._Bears.Length <= 1)
            {
                value += 3;
            }

            if (Sally != null)
            {
                if (Sally._Saturation == 0)
                {
                    value += 20;
                }

                if(Kast == Kasta.Первопроходец)
                {
                    value -= 10;
                }

                return value;
            }

            if (City._Time._WorldTime / 1500 > 70)
            {
                value += 10;
            }
            else if (City._Time._WorldTime / 1500 > 50)
            {
                value -= 5;
            }
            else if (City._Time._WorldTime / 1500 > 40)
            {
                value += 5;
            }
            else if (City._Time._WorldTime / 1500 > 30)
            {
                value += 0;
            }
            else if (City._Time._WorldTime / 1500 > 20)
            {
                value -= 5;
            }
            else if (City._Time._WorldTime / 1500 > 10)
            {
                value -= 5;
            }
            else if (City._Time._WorldTime / 1500 > 5)
            {
                value -= 10;
            }
            else
            {
                value -= 15;
            }

            if (Home == null)
            {
                value += 10;
            }

                if (City._Foodstream._Saturation == 0)
            {
                value += 20;
            }
            else if (City._Foodstream._Saturation < 1)
            {
                value += 3;
            }
            else
            {
                    value -= 3;
            }

            if (Tired >= 0.75)
            {
                value += 5;
            }

            if (Health < 4)
            {
                value += 3;
            }
            else if (Health > 7)
            {
                value -= 2;
            }

            if (Facility == null)
            {
                value += 5;
            }

            if (CurrentFacility != null && !(CurrentFacility is Home))
            {
                if (CurrentFacility._RequiredKasta != Kast)
                {
                    value += 10;
                }
            }

            if (CurrentFacility is Home)
            {
                value -= (CurrentFacility as Home)._StressDown;
            }

            return value;
        }
    }
    public virtual float _Work
    {
        get
        {
            if(_Nedeesposoben)
            {
                return 0;
            }

            float value = 1;

            if(Sally == null)
            {
                value *= Mathf.Clamp(City._Foodstream._Saturation, 0.5f, 1.5f);

                if (Tired >= 1)
                {
                    value *= 0.1f;
                }
            }
            else
            {
                value *= Mathf.Clamp(Sally._Saturation, 0.5f, 1.5f);
            }

            if (Stress > 50)
            {
                value *= 0.9f;
            }

            if (Health >= 5)
            {
                value *= 1 + Health * 0.025f;
            }
            else
            {
                value *= Health / 5f;
            }

            if (Facility != null)
            {
                if (Facility._RequiredKasta != Kast && Facility._RequiredKasta != Kasta.Неопределено)
                {
                    value *= 0.5f;
                }
            }

            if (Effects[0] > 0)
            {
                value *= 0.9f;
            }
            if (Effects[2] > 0)
            {
                value *= 1.33f;
            }

            return value;
        }
    }

    public int _Schedule
    {
        get
        {
            return Schedule;
        }
        set
        {
            Schedule = value;
            OnSmthChange?.Invoke();
        }
    }

    public bool _Nedeesposoben => Health == 1 || Effects[3] <= 0;
    
    public CitySally.Sally _Sally
    {
        get
        {
            return Sally;
        }
        set
        {
            Sally = value;

            if (Sally != null)
            {
                AtWork = false;

                if(Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y)
                {
                    if(Health > 1)
                    {
                        if (Object == null)
                        {
                            Object = City._BearObjectioner.Create(this);

                            if (CurrentFacility != null)
                            {
                                Object.transform.position = CurrentFacility._EnterPoint.position;
                            }
                            else
                            {
                                Object.transform.position = GetRandomPosition();
                            }

                            Object._NavMeshAgent.Warp(Object.transform.position);
                        }

                        Object.SetTask(new BearObject.SallyTask(Object, "Ожидание вылазки", Sally));
                    }

                    if (CurrentFacility != null)
                    {
                        CurrentFacility.BearArrived(this, true);
                    }

                    if (Facility != null)
                    {
                        Facility.AssignBear(this, true);
                    }
                    if (Home != null)
                    {
                        Home.AssignBear(this, true);
                    }
                }
            }
            else
            {
                foreach(Facility facility in City._DataBase._Facilities)
                {
                    if(facility is Home && facility._AssignedBears.Length < facility._MaxBearCount)
                    {
                        facility.AssignBear(this, false);
                        break;
                    }
                }
                FollowSchedule(City._CitySchedule._Schedules[Schedule]._Work);
            }
        }
    }

    public int _Face => Face;
    public int _Brows => Brows;
    public int _BodyColor => BodyColor;

    public int _HeatLevel
    {
        get
        {
            if(Sally != null)
            {
                return 3 + Mathf.RoundToInt(2 * Sally._Saturation) + (City._Research.GetResearchLevel(CityResearch.ResearchType.Travels) >= 1 ? 3 : 0) - City._Weather._Cold;
            }

            return Mathf.RoundToInt(2 * City._Foodstream._Saturation) + (CurrentFacility != null ? CurrentFacility._ColdEndurance : 0) - City._Weather._Cold;
        }
    }
    public int[] _Effects
    {
        get
        {
            return Effects;
        }
        set
        {
            Effects = value;
        }
    }
    public virtual string _SaveInfo
    {
        get
        {
            return $"Kast({Kast.GetHashCode()})HP({Health})Tired({Tired})Stress({Stress})Age({Age})Name({Name})Look({Face};{Brows};{BodyColor})Effs({Effects[0]};{Effects[1]};{Effects[2]};{Effects[3]};{Effects[4]})";
        }
        set
        {
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            Kast = (Kasta)int.Parse(parameters["Kast"]);
            Health = int.Parse(parameters["HP"]);
            Tired = float.Parse(parameters["Tired"]);
            Stress = float.Parse(parameters["Stress"]);
            Age = int.Parse(parameters["Age"]);
            Name = parameters["Name"];

            string[] info = parameters["Effs"].Split(';');
            Effects = new int[] { int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]), int.Parse(info[3]), int.Parse(info[4]) };

            info = parameters["Look"].Split(';');
            Face = int.Parse(info[0]);
            Brows = int.Parse(info[1]);
            BodyColor = int.Parse(info[2]);
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
            if (Sally != null)
            {
                Home = null;
                return;
            }

            Home = value;

            FollowSchedule(AtWork);

            if (OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }
    public Facility _CurrentFacility
    {
        get
        {
            return CurrentFacility;
        }
        set
        {
            CurrentFacility = value;
            OnSmthChange?.Invoke();
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
            if (Sally != null)
            {
                Facility = null;
                return;
            }

            Facility = value;

            FollowSchedule(AtWork);

            if (OnSmthChange != null)
            {
                OnSmthChange.Invoke();
            }
        }
    }
    public BearObject _BearObject
    {
        get
        {
            return Object;
        }
        set
        {
            Object = value;
        }
    }

    public void FollowSchedule(bool work)
    {
        if(Sally != null)
        {
            AtWork = false;
            return;
        }

        AtWork = work;

        if (AtWork)
        {
            if(Facility != null)
            {
                ArriveFacility(Facility);
            }
            else
            {
                ArriveFacility(Home);
            }
        }
        else
        {
            ArriveFacility(Home);
        }
    }

    private void ArriveFacility(Facility facility)
    {
        if (CurrentFacility == facility && CurrentFacility != null && facility != null)
        {
            return;
        }

        if (!City._CitySchedule._Initialized && facility != null || Health == 1)
        { 
            if (CurrentFacility != null)
            {
                CurrentFacility.BearArrived(this, true);
            }

            CurrentFacility = facility;

            if (CurrentFacility != null)
            {
                CurrentFacility.BearArrived(this, false);
            }
            return;
        }

        if (Object == null)
        {
            Object = City._BearObjectioner.Create(this);

            if(CurrentFacility != null)
            {
                Object.transform.position = CurrentFacility._EnterPoint.position;
            }
            else
            {
                Object.transform.position = GetRandomPosition();
            }

            Object._NavMeshAgent.Warp(Object.transform.position);
        }

        if (CurrentFacility != null)
        {
            CurrentFacility.BearArrived(this, true);
        }

        CurrentFacility = facility;

        if (CurrentFacility != null)
        {
            CurrentFacility.BearArrived(this, false);
        }

        if (facility != null)
        {
            Object._NavMeshAgent.speed = UnityEngine.Random.Range(2.5f, 4f);
            Object.SetTask(new BearObject.DestinateTask(Object, $"Направляется на {facility._ConstructInfo.Name}", facility));
        }
        else
        {
            if(!(Object._Task is BearObject.TalkTask))
            {
                Object.SetTask(new BearObject.RandomWalkTask(Object, "Бродит"));
            }
        }
    }

    public virtual  CityFactors.Factor[] GetWorkFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[3];

        factors[0] = new CityFactors.Factor("Базовое значение", 1);

        if(Sally != null)
        {
            factors[1] = new CityFactors.Factor("Коэффициент сытости", Mathf.Clamp(Sally._Saturation, 0.5f, 1.5f), true);
        }
        else
        {
            factors[1] = new CityFactors.Factor("Коэффициент сытости", Mathf.Clamp(City._Foodstream._Saturation, 0.5f, 1.5f), true);

            if (Tired >= 1)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Безумно устал", 0.1f, true));
            }
        }

        
        if (Stress > 50)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Высокий стресс", 0.9f, true));
        }

        float healthCoef = 1;
        if (Health >= 5)
        {
            healthCoef = 1 + Health * 0.025f;
        }
        else
        {
            healthCoef = Health / 5f;
        }

        factors[2] = new CityFactors.Factor("Коэффициент здоровья", healthCoef, true);

        if (Facility != null)
        {
            if (Facility._RequiredKasta != Kast && Facility._RequiredKasta != Kasta.Неопределено)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Работа не по специальности", 0.5f  , true));
            }
        }

        if (Effects[0] > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Успокивающие", 0.9f, true));
        }
        if (Effects[2] > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Стимулятор", 1.33f,true));
        }

        return factors;
    }

    public virtual CityFactors.Factor[] GetStressFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        if (_HeatLevel < -2)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Собачий холод", 6  , false));
        }
        else if (_HeatLevel < 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Холодно", 6, false));
        }
        else
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Тепло", -1, false));
        }

        if (Effects[0] > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Успокаивающие", -10, false));
        }
        if (Effects[4] > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Спасён", -40, false));
        }

        if (CurrentFacility != null && CurrentFacility._Bears.Length <= 1)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Одиночество", 3, false));
        }

        if (Sally != null)
        {
            if (Sally._Saturation == 0)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Голод", 20, false));
            }

            if (Kast == Kasta.Первопроходец)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Обожает вылазки", -10, false));
            }

            return factors;
        }


        if (City._Time._WorldTime / 1500 > 70)
        {
            factors[0] = new Factor("Заберите меня домой", 10, false);
        }
        else if(City._Time._WorldTime / 1500 > 50)
        {
            factors[0] = new Factor("Сейчас наша цель - выжить !", -5, false);
        }
        else if (City._Time._WorldTime / 1500 > 40)
        {
            factors[0] = new Factor("Тоска по родному дому", 5, false);
        }
        else if (City._Time._WorldTime / 1500 > 30)
        {
            factors[0] = new Factor("Тоска по родному дому", 0, false);
        }
        else if (City._Time._WorldTime / 1500 > 20)
        {
            factors[0] = new Factor("Сейчас не время унывать !", -5, false);
        }
        else if (City._Time._WorldTime / 1500 > 10)
        {
            factors[0] = new Factor("Надо взять себя в руки !", -5, false);
        }
        else if (City._Time._WorldTime / 1500 > 5)
        {
            factors[0] = new Factor("Надо взять себя в руки !", -10, false);
        }
        else
        {
            factors[0] = new Factor("Надо взять себя в руки !", -15, false);
        }

        if(Home == null)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Отсутствие жилья", 10, false));
        }


        if (City._Foodstream._Saturation == 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Голод", 20, false));
        }
        else if(City._Foodstream._Saturation < 1)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Недоедание", 5, false));
        }
        else
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Сытость", -3, false));
        }

        if (Tired >= 0.75)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Устал", 5, false));
        }

        if (Health < 4)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Плохо себя чувствует", 2, false));
        }
        else if(Health > 7)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("В здоровом теле здоровый дух", -2, false));
        }

        if(Facility == null)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Безработный", 3, false));
        }

        if (CurrentFacility != null && !(CurrentFacility is Home))
        {
            if (CurrentFacility._RequiredKasta != Kast)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Работа не по специальности", 10, false));
            }
        }

        if(CurrentFacility is Home)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Домашний досуг", -(CurrentFacility as Home)._StressDown));
        }

        return factors;
    }

    public static Vector3 GetRandomPosition()
    {
        Vector3 position;

        do
        {
            position = new Vector3(UnityEngine.Random.Range(-45f, 45f), 0, UnityEngine.Random.Range(-45f, 45f));

            if (Physics.Raycast(position + Vector3.up * 50, Vector3.down, out RaycastHit hit, 200, 128))
            {
                position = hit.point;
            }
        }
        while (Physics.CheckSphere(position, 0.1f, 256));

        return position;
    }

    public Bear() 
    {

    }

    public Bear(Kasta kast, string name, int health, int age, int face, int brows, int color, int antisleep)
    {
        Kast = kast;
        Name = name;
        Health = health;
        Age = age;
        Face = face;
        Brows = brows;
        BodyColor = color;
        Effects = new int[] { 0, 0, 0, antisleep, 0};
    }
}

[Serializable]
public class SuperBear : Bear
{
    protected float WorkMultiplier;
    protected float StressOffset;
    protected float Restful;
    protected float Regeneration;

    public float _WorkMultiplier => WorkMultiplier;
    public float _StressOffset => StressOffset;
    public float _Restful => Restful;
    public float _Regeneration => Regeneration;

    public override float _StressDelta => base._StressDelta + StressOffset;
    public override float _Work => base._Work * WorkMultiplier;
    public override string _SaveInfo 
    { 
        get => base._SaveInfo + $"WorkMP({WorkMultiplier})StressOff({StressOffset})Rest({Restful})Regen({Regeneration})";
        set
        {
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            Kast = (Kasta)int.Parse(parameters["Kast"]);
            Health = int.Parse(parameters["HP"]);
            Tired = float.Parse(parameters["Tired"]);
            Stress = float.Parse(parameters["Stress"]);
            Age = int.Parse(parameters["Age"]);
            Name = parameters["Name"];

            string[] info = parameters["Effs"].Split(';');
            Effects = new int[] { int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]), int.Parse(info[3]), int.Parse(info[4]) };

            info = parameters["Look"].Split(';');
            Face = int.Parse(info[0]);
            Brows = int.Parse(info[1]);
            BodyColor = int.Parse(info[2]);

            WorkMultiplier = StaticTools.StringToFloat(parameters["WorkMP"]);
            StressOffset = StaticTools.StringToFloat(parameters["StressOff"]);
            Restful = StaticTools.StringToFloat(parameters["Rest"]);
            Regeneration = StaticTools.StringToFloat(parameters["Regen"]);
        }
    }
    public override Factor[] GetStressFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        if (_HeatLevel < -2)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Собачий холод", 6, false));
        }
        else if (_HeatLevel < 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Холодно", 6, false));
        }
        else
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Тепло", -1, false));
        }

        if (Effects[0] > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Успокаивающие", -10, false));
        }
        if (Effects[4] > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Спасён", -40, false));
        }

        if (CurrentFacility != null && CurrentFacility._Bears.Length <= 1)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Одиночество", 3, false));
        }

        if (Sally != null)
        {
            if (Sally._Saturation == 0)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Голод", 20, false));
            }

            if (Kast == Kasta.Первопроходец)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Обожает вылазки", -10, false));
            }

            return factors;
        }


        if (City._Time._WorldTime / 1500 > 70)
        {
            factors[0] = new Factor("Заберите меня домой", 10, false);
        }
        else if (City._Time._WorldTime / 1500 > 50)
        {
            factors[0] = new Factor("Сейчас наша цель - выжить !", -5, false);
        }
        else if (City._Time._WorldTime / 1500 > 40)
        {
            factors[0] = new Factor("Тоска по родному дому", 5, false);
        }
        else if (City._Time._WorldTime / 1500 > 30)
        {
            factors[0] = new Factor("Тоска по родному дому", 0, false);
        }
        else if (City._Time._WorldTime / 1500 > 20)
        {
            factors[0] = new Factor("Сейчас не время унывать !", -5, false);
        }
        else if (City._Time._WorldTime / 1500 > 10)
        {
            factors[0] = new Factor("Надо взять себя в руки !", -5, false);
        }
        else if (City._Time._WorldTime / 1500 > 5)
        {
            factors[0] = new Factor("Надо взять себя в руки !", -10, false);
        }
        else
        {
            factors[0] = new Factor("Надо взять себя в руки !", -15, false);
        }

        if (Home == null)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Отсутствие жилья", 10, false));
        }

        if (City._Foodstream._Saturation == 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Голод", 20, false));
        }
        else if (City._Foodstream._Saturation < 1)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Недоедание", 5, false));
        }
        else
        {
                factors = StaticTools.ExpandMassive(factors, new Factor("Сытость", -3, false));
        }

        if (Tired >= 0.75)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Устал", 5, false));
        }

        if (Health < 4)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Плохо себя чувствует", 2, false));
        }
        else if (Health > 7)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("В здоровом теле здоровый дух", -2, false));
        }

        if (Facility == null)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Безработный", 3, false));
        }

        if (CurrentFacility != null && !(CurrentFacility is Home))
        {
            if (CurrentFacility._RequiredKasta != Kast)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Работа не по специальности", 10, false));
            }
        }

        if (CurrentFacility is Home)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Домашний досуг", -(CurrentFacility as Home)._StressDown));
        }

        factors = StaticTools.ExpandMassive(factors, new Factor("Характер медведя", Restful));

        return factors;
    }
    public override Factor[] GetWorkFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[3];

        factors[0] = new CityFactors.Factor("Базовое значение", 1);

        if (Sally != null)
        {
            factors[1] = new CityFactors.Factor("Коэффициент сытости", Mathf.Clamp(Sally._Saturation, 0.5f, 1.5f), true);
        }
        else
        {
            factors[1] = new CityFactors.Factor("Коэффициент сытости", Mathf.Clamp(City._Foodstream._Saturation, 0.5f, 1.5f), true);

            if (Tired >= 1)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Безумно устал", 0.1f, true));
            }
        }


        if (Stress > 50)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Высокий стресс", 0.9f, true));
        }

        float healthCoef = 1;
        if (Health >= 5)
        {
            healthCoef = 1 + Health * 0.025f;
        }
        else
        {
            healthCoef = Health / 5f;
        }

        factors[2] = new CityFactors.Factor("Коэффициент здоровья", healthCoef, true);

        if (Facility != null)
        {
            if (Facility._RequiredKasta != Kast && Facility._RequiredKasta != Kasta.Неопределено)
            {
                factors = StaticTools.ExpandMassive(factors, new Factor("Работа не по специальности", 0.5f, true));
            }
        }

        if (Effects[0] > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Успокивающие", 0.9f, true));
        }
        if (Effects[2] > 0)
        {
            factors = StaticTools.ExpandMassive(factors, new Factor("Стимулятор", 1.33f, true));
        }

        factors = StaticTools.ExpandMassive(factors, new Factor("Характер медведя", WorkMultiplier, true));

        return factors;
    }

    public SuperBear()
    {

    }

    public SuperBear(UserContent.CustomBear customBear)
    {
        Name = customBear.Name;
        Kast = customBear.Kasta;
        Health = customBear.Health;
        Age = customBear.Age;
        Face = customBear.Face;
        Brows = customBear.Brows;
        BodyColor = customBear.BodyColor;

        WorkMultiplier = customBear.WorkMultiplier;
        StressOffset = customBear.Stressful;
        Restful = customBear.Restful;
        Regeneration = customBear.Regeneration;

        if (City._Instance != null)
        {
            Effects = new int[] { 0, 0, 0, Age >= 20 ? Mathf.Max(100, UnityEngine.Random.Range(900, 1000) - (int)(City._Time._WorldTime / 60)) : 9125 * Mathf.Abs(20 - Age), 0 };
        }
        else
        {
            Effects = new int[] { 0, 0, 0, 1000, 0 };
        }
    }
}