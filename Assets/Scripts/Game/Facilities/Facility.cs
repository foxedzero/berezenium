using System;
using UnityEngine;
using static Constructor;

public class Facility : MonoBehaviour, IInteractable
{
    [SerializeField] protected AudioClip IntroSound;
    [SerializeField] protected Collider[] Collider;
    [SerializeField] protected int BearCount;

    [SerializeField] protected Bear.Kasta RequiredKasta;
    [SerializeField] protected float RequiredSkill;

    [SerializeField] protected float RequiredEnergy;
    [SerializeField] protected float MinimalEnergyCoeffiente;

    [SerializeField] protected int ColdEndurance;

    protected bool Enabled = true;
    protected int HeaterOn = 0;
    protected int Heated = 0;

    [SerializeField] protected Bear[] AssignedBears;

    public Bear.Kasta _RequiredKasta => RequiredKasta;
    public virtual int _MaxBearCount => BearCount;
    public float _RequiredSkill => RequiredSkill;

    public Bear[] _AssignedBears => AssignedBears;
    protected ConstructInfo ConstructInfo = null;

    public event SimpleVoid OnFacilityDestroy = null;
    public virtual event SimpleVoid OnSmthChange = null;

    public ConstructInfo _ConstructInfo
    {
        get
        {
            return ConstructInfo;
        }
        set
        {
            ConstructInfo = value;
        }
    }

    public virtual bool _CanBeDisabled => true;
    public virtual bool _CanBeHeated => true;

    public float _MinimalEnergyCoefficiente => MinimalEnergyCoeffiente;
    public float _EnergyConsume => RequiredEnergy;
    public virtual int _ColdEndurance => ColdEndurance + Heated;
    public virtual float _Effectivity
    {
        get
        {
            float summSkill = 0;
            foreach(Bear bear in AssignedBears)
            {
                summSkill += bear._Skill;
            }

            return Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f) * Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) * (summSkill < _RequiredSkill ? 0.5f : 1) * (City._Buyiments._EffectivityBoost ? 1.25f : 1);
        }
    }
    public virtual bool _Enabled
    {
        get
        {
            return Enabled;
        }
        set
        {
            Enabled = value;

            OnSmthChange?.Invoke();
        }
    }
    public virtual int _Heater
    {
        get
        {
            return HeaterOn;
        }
        set
        {
            HeaterOn = Mathf.Clamp(value, 0, 3);

            if (HeaterOn > 0)
            {
                City._CityHeater.ApplyFacilityHeater(this, false);
            }
            else
            {
                City._CityHeater.ApplyFacilityHeater(this, true);
            }

            OnSmthChange?.Invoke();
        }
    }
    public virtual int _Heated
    {
        get
        {
            return Heated;
        }
        set
        {
            Heated = value;

            OnSmthChange?.Invoke();
        }
    }
    public virtual string _SaveInfo
    {
        get
        {
            string info = $"Type({ConstructInfo.Prefab.name})Transform({Mathf.FloorToInt(transform.position.x)};{Mathf.FloorToInt(transform.position.z)};{Mathf.RoundToInt(transform.localEulerAngles.y / 90)})Enabled({Enabled.GetHashCode()})Heater({HeaterOn.GetHashCode()})Bears(";

            foreach(Bear bear in AssignedBears)
            {
                info += $"{Array.IndexOf(City._DataBase._Bears, bear)};";
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
            string bearses = StaticTools.GetParameter(value, "Bears");

            _Enabled = StaticTools.GetParameter(value, "Enabled") == "1";
            _Heater = StaticTools.StringToInt(StaticTools.GetParameter(value, "Heater"));

            if (bearses.Length > 0)
            {
                string[] bears = bearses.Split(";");
                AssignedBears = new Bear[bears.Length];

                for (int i = 0; i < bears.Length; i++)
                {
                    AssignedBears[i] = City._DataBase._Bears[int.Parse(bears[i])];
                    AssignedBears[i]._Facility = this;
                }
            }

        }
    }

    protected virtual void Start()
    {
        foreach(Collider collider in Collider)
        {
            collider.enabled = false;
        }
        foreach (Collider collider in Collider)
        {
            collider.enabled = true;
        }
    }

    protected virtual void OnDestroy()
    {
        City._DataBase.RegisterFacility(this, true);

        foreach(Bear bear in AssignedBears)
        {
           bear._Facility = null;
        }

        OnFacilityDestroy?.Invoke();
    }

    public virtual float GetPotencialEffectivity()
    {
        float summSkill = 0;
        foreach (Bear bear in AssignedBears)
        {
            summSkill += bear.GetSkill(City._Foodstream._Potencial[2]);
        }

        return Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f) * Mathf.Clamp(Mathf.Pow(City._Energosystem._Potencial[2], 2), MinimalEnergyCoeffiente, 2)   * (summSkill < _RequiredSkill ? 0.5f : 1) * (City._Buyiments._EffectivityBoost ? 1.25f : 1);
    }
    public virtual CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[2];

        float summSkill = 0;
        foreach (Bear bear in AssignedBears)
        {
            summSkill += bear._Skill;
        }

        factors[0] = new CityFactors.Factor($"Опыт медведей", Mathf.Clamp(summSkill / _RequiredSkill, 0, 1.5f), -1, true);
        factors[1] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), -1, true);

        if(summSkill < _RequiredSkill)
        {
            factors =  StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Неопытность сотрудников", 0.5f, -1, true));
        }

        if (City._Buyiments._EffectivityBoost)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Промышленные нановнедрения", 1.25f, -1, true));
        }

        return factors;
    }

    public virtual void Interact()
    {
        SoundEffector.PlayFasilityIntro(IntroSound);
        UserInteract.AskVariants($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", new string[] {"Выбрать", "Дать распоряжение"}, new int[] {0, 1}, Interact);
        
    }
    public virtual void Interact(int index)
    {
        switch (index)
        {
            case 0:
                FindObjectOfType<WindowCreator>().CreateWindow<FacilityWindow>().SetInfo(this);
                break;
            case 1:
                RightMouse();
                break;
        }
    }

    public virtual bool AssignBear(Bear bear, bool remove)
    {
        int index = StaticTools.IndexOf(AssignedBears, bear);
        if (remove)
        {
            if(index > -1)
            {
                AssignedBears = StaticTools.ReduceMassive(AssignedBears, index);

                bear._Facility = null;

                if(OnSmthChange != null)
                {
                    OnSmthChange.Invoke();
                }

                return true;
            }
        }
        else
        {
            if(index < 0 && AssignedBears.Length < _MaxBearCount)
            {
                AssignedBears = StaticTools.ExpandMassive(AssignedBears, bear);

                if(bear._Facility != null)
                {
                    bear._Facility.AssignBear(bear, true);
                }

                bear._Facility = this;

                if (OnSmthChange != null)
                {
                    OnSmthChange.Invoke();
                }

                return true;
            }
        }

        return false;
    }

    public virtual void GiveExperience()
    {
        float value = 10;
        foreach(Bear bear in AssignedBears)
        {
            value *= (2 - Mathf.Clamp01(Mathf.InverseLerp(14, 30, bear._Age)));

            if(bear._Kasta == Bear.Kasta.Неопределёно)
            {
                if(UnityEngine.Random.Range(0, 100) < 30)
                {
                    bear._Kasta = RequiredKasta;
                }
            }
            else if (RequiredKasta != bear._Kasta)
            {
                value /= 3f;
            }

            bear.SkillUp(Mathf.RoundToInt( value));
        }
    }

    public virtual void RightMouse()
    {
        string[] variants = new string[] {"Снести"};
        int[] indexes = new int[] {0};

        if (_CanBeDisabled)
        {
            variants = StaticTools.ExpandMassive(variants, (_Enabled ? "Приостановить работу" : "Возобновить работу"));
            indexes = StaticTools.ExpandMassive(indexes, 1);
        }

        if (_CanBeHeated && City._Research.GetResearchLevel(CityResearch.ResearchType.Cold) >= 1)
        {
            variants = StaticTools.ExpandMassive(variants, "Установить режим работы обогревателя");
            indexes = StaticTools.ExpandMassive(indexes, 2);
        }

        if(_MaxBearCount > 0)
        {
            variants = StaticTools.ExpandMassive(variants, "Автоматически назначить");
            indexes = StaticTools.ExpandMassive(indexes, 3);

            if (AssignedBears.Length > 0)
            {
                variants = StaticTools.ExpandMassive(variants, "Снять всех с назначения");
                indexes = StaticTools.ExpandMassive(indexes, 4);
            }
        }

        UserInteract.AskVariants("Распоряжение", variants, indexes, RightMouseActions);
    }
    public virtual void RightMouseActions(int index)
    {
        switch (index)
        {
            case 0:
                UserInteract.AskConfirm("Снос", $"Вы собираетесь дать распоряженио о сносе данного строения.\nПри сносе здания вы получите половину от его стоимости строительства (древесина: {ConstructInfo.WoodCost / 2f}, металл: {ConstructInfo.MetalCost / 2f}, березениум: {ConstructInfo.BerezenuimCost / 2f}).\nВы уверены ?", Deconstruct);
                break;
            case 1:
                _Enabled = !Enabled;
                SoundEffector.PlayEnableDisable(_Enabled);
                break;
            case 2:
                UserInteract.AskVariants("Режим обогревателя", new string[] {"Электричество", "Энергомёд", "Древесина", "Выключен"}, new int[] {1, 2, 3, 0}, SetHeater);
                break;
            case 3:
                foreach(Bear bear in AssignedBears)
                {
                    bear._Facility = null;
                }
                AssignedBears = new Bear[0];

                Bear[] bears = City._DataBase._Bears;
                float skill = 0;

                while(AssignedBears.Length < _MaxBearCount)
                {
                    int maximal = -1;
                    for(int i = 0; i < bears.Length; i++)
                    {
                        if (bears[i]._Kasta == _RequiredKasta && bears[i]._Facility == null && !StaticTools.Contains(AssignedBears, bears[i]))
                        {
                            if(maximal == -1)
                            {
                                maximal = i;
                            }
                            else if (bears[maximal]._Skill < bears[i]._Skill)
                            {
                                maximal = i;
                            }
                        }
                    }

                    if(maximal == -1)
                    {
                        break;
                    }

                    skill += bears[maximal]._Skill;
                    bears[maximal]._Facility = this;
                    AssignedBears = StaticTools.ExpandMassive(AssignedBears, bears[maximal]);
                }

                OnSmthChange?.Invoke();
                break;
            case 4:
                foreach (Bear bear in AssignedBears)
                {
                    bear._Facility = null;
                }
                AssignedBears = new Bear[0];

                OnSmthChange?.Invoke();
                break;
        }
    }
    public virtual void SetHeater(int index)
    {
        _Heater = index;
        SoundEffector.PlayEnableDisable(index > 0);
        OnSmthChange?.Invoke();
    }
    public virtual void Deconstruct(bool answer)
    {
        if (answer)
        {
            City._Storage._Wood += ConstructInfo.WoodCost / 2f;
            City._Storage._Metal += ConstructInfo.MetalCost / 2f;
            City._Storage._Berezenium += ConstructInfo.BerezenuimCost / 2f;
            Destroy(gameObject);
            City._Constructor.SpawnDemolishEffect(transform.position);
        }
    }
}
