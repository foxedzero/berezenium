
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Constructor;

public class Facility : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject Ocantovka;
    [SerializeField] private Outline Outline;
    [SerializeField] private Transform EnterPoint;
    [SerializeField] protected Text OcantovkaInfo;

    [SerializeField] protected AudioClip IntroSound;
    [SerializeField] protected Collider[] Collider;
    [SerializeField] protected int BearCount;

    [SerializeField] protected Bear.Kasta RequiredKasta;

    [SerializeField] protected float RequiredEnergy;
    [SerializeField] protected float MinimalEnergyCoeffiente;

    [SerializeField] protected float Tiring;

    [SerializeField] protected int ColdEndurance;

    [SerializeField] protected LayerMask TreeMask; 

    protected int HeaterOn = 0;
    protected int Heated = 0;

    [SerializeField] protected Bear[] AssignedBears;
    [SerializeField] protected Bear[] Bears;

    protected ConstructInfo ConstructInfo = null;

    public Bear.Kasta _RequiredKasta => RequiredKasta;
    public virtual int _MaxBearCount => BearCount;

    public string _Info => "";

    public Bear[] _AssignedBears => AssignedBears;
    public Bear[] _Bears => Bears;

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

    public virtual bool _CanBeHeated => true;

    public Transform _EnterPoint => EnterPoint;

    public float _MinimalEnergyCoefficiente => MinimalEnergyCoeffiente;
    public virtual float _EnergyConsume => Bears.Length == 0 ? 0 : RequiredEnergy;
    public virtual float _BaseEnergyConsume => RequiredEnergy;
    public virtual int _ColdEndurance => ColdEndurance + Heated;
    public virtual float _Tiring => Tiring;
    public virtual float _Effectivity
    {
        get
        {
            float work = 0;
            foreach(Bear bear in Bears)
            {
                work += bear._Work;
            }

            return  Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) ;
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
            HeaterOn = Mathf.Clamp(value, 0, 2);

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
    public virtual int _Order => 0;
    public virtual string _SaveInfo
    {
        get
        {
            string info = $"Type({ConstructInfo.Prefab.name})Transform({Mathf.FloorToInt(transform.position.x)};{Mathf.FloorToInt(transform.position.z)};{Mathf.RoundToInt(transform.localEulerAngles.y / 90)})Heater({HeaterOn.GetHashCode()})Bears(";

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
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            ApplySaveInfo(parameters);
        }
    }

    public bool _AbstractUse => true;

    protected virtual void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        string bearses = parameters["Bears"];

        _Heater = StaticTools.StringToInt(parameters["Heater"]);

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

    protected virtual void Start()
    {
        if(Physics.Raycast(EnterPoint.position + Vector3.up * 50, Vector3.down, out RaycastHit hit, 150, 128))
        {
            EnterPoint.position = hit.point;
        }

        foreach (Collider collider in Collider)
        {
            collider.enabled = false;
        }
        foreach (Collider collider in Collider)
        {
            collider.enabled = true;
        }

        StartCoroutine(DestroyInside());
    }

    public void ResetHeated()
    {
        Heated = 0;
    }

    public virtual void HourPassed()
    {
        if (Bears.Length <= 0)
        {
            return;
        }

        switch (HeaterOn)
        {
            case 1:
                if (City._Storage._EnergyHoney >= 1)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Обогреватель {ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.EnergyHoney));
                    City._Storage._EnergyHoney -= 1;
                    Heated += 3;
                }
                break;
            case 2:
                if (City._Storage._Wood >= 5)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"Обогреватель {ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", (int)(City._Time._WorldTime / 60), -5, CityStorage.ResourceType.Wood));
                    City._Storage._Wood -= 5;
                    Heated += 2;
                }
                break;
        }

        foreach (Bear bear in Bears)
        {
            bear._Tired += CityTime._DaySection * Tiring * bear._TiredCoefficient;
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

    public virtual CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        float work = 0;
        foreach (Bear bear in Bears)
        {
            work += bear._Work;
        }

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), true);

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0,  true));
        }

        return factors;
    }

    public virtual void Interact()
    {
        SoundEffector.PlayFasilityIntro(IntroSound);

        string[] variants = new string[] { "Информация", "Снести" };
        int[] indexes = new int[] { -1, 0 };

        if (_CanBeHeated && City._Research.GetResearchLevel(CityResearch.ResearchType.Cold) >= 1)
        {
            variants = StaticTools.ExpandMassive(variants, "Установить режим работы обогревателя");
            indexes = StaticTools.ExpandMassive(indexes, 2);
        }

        if (_MaxBearCount > 0)
        {
            variants = StaticTools.ExpandMassive(variants, "Автоматически назначить");
            indexes = StaticTools.ExpandMassive(indexes, 3);

            if (AssignedBears.Length > 0)
            {
                variants = StaticTools.ExpandMassive(variants, "Снять всех с назначения");
                indexes = StaticTools.ExpandMassive(indexes, 4);
            }
        }

        UserInteract.AskVariants($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", variants, indexes, RightMouseActions);
    }

    public virtual void RightMouseActions(int index)
    {
        switch (index)
        {
            case -1:
                WindowCreator.CreateWindow<FacilityWindow>().SetInfo(this);
                break;
            case 0:
                UserInteract.AskConfirm("Снос", $"Вы собираетесь дать распоряжение о сносе данного строения.\nПри сносе здания вы получите половину от его стоимости строительства (древесина: {ConstructInfo.WoodCost / 2f}, металл: {ConstructInfo.MetalCost / 2f}, березениум: {ConstructInfo.BerezenuimCost / 2f}).\nВы уверены ?", Deconstruct);
                break;
            case 2:
                UserInteract.AskVariants("Режим обогревателя", new string[] {  "Энергомёд", "Древесина", "Выключен" }, new int[] { 1, 2, 0 }, SetHeater);
                break;
            case 3:
                AutoAssign();
                break;
            case 4:
                Unassign();
                break;
        }
    }

    public virtual void Indicate(bool state)
    {
        if (Ocantovka != null)
        {
            Ocantovka.SetActive(state);
            Outline.enabled = state;

            if (state)
            {
                OnSmthChange += UpdateOcantovkaInfo;
                UpdateOcantovkaInfo();
            }
            else
            {
                OnSmthChange -= UpdateOcantovkaInfo;
            }
        }
        else
        {
            Destroy(this);
        }

    }

    protected virtual void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////\nОбъект: {ConstructInfo.Name}\nУровень тепла: {(_Heated < 0 ? $"<color=red>{_Heated}</color>" : _Heated)}" +
            $"\nМедведей: {(AssignedBears.Length == 0 ? $"<color=red>{AssignedBears.Length}/{_MaxBearCount}</color>" : $"{AssignedBears.Length}/{_MaxBearCount}")}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}\n/////////";
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
            if(bear._Sally != null)
            {
                return false;
            }

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
    public virtual bool BearArrived(Bear bear, bool leave)
    {
        int index = StaticTools.IndexOf(Bears, bear);
        if (leave)
        {
            if (index > -1)
            {
                Bears = StaticTools.ReduceMassive(Bears, index);

                if (OnSmthChange != null)
                {
                    OnSmthChange.Invoke();
                }

                return true;
            }
        }
        else
        {
            if (bear._Sally != null)
            {
                return false;
            }

            if (index < 0 && Bears.Length < _MaxBearCount)
            {
                Bears = StaticTools.ExpandMassive(Bears, bear);

                if (OnSmthChange != null)
                {
                    OnSmthChange.Invoke();
                }

                return true;
            }
        }

        return false;
    }

    public virtual void Unassign()
    {
        foreach (Bear bear in AssignedBears)
        {
            bear._Facility = null;
        }
        AssignedBears = new Bear[0];

        OnSmthChange?.Invoke();
    }

    public virtual void AutoAssign()
    {
        foreach (Bear bear in AssignedBears)
        {
            bear._Facility = null;
        }
        AssignedBears = new Bear[0];

        Bear[] bears = City._DataBase._Bears;

        while (AssignedBears.Length < _MaxBearCount)
        {
            int maximal = -1;
            for (int i = 0; i < bears.Length; i++)
            {
                if (bears[i]._Kasta == _RequiredKasta && bears[i]._Facility == null && !StaticTools.Contains(AssignedBears, bears[i]) && bears[i]._Sally == null)
                {
                    if (maximal == -1)
                    {
                        maximal = i;
                    }
                }
            }

            if (maximal == -1)
            {
                break;
            }

            bears[maximal]._Facility = this;
            AssignedBears = StaticTools.ExpandMassive(AssignedBears, bears[maximal]);
        }

        OnSmthChange?.Invoke();
    }
    protected void SmtChanged()
    {
        if(gameObject != null)
        {
            OnSmthChange?.Invoke();
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
            if (ConstructInfo.WoodCost > 0)
            {
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"отмена проекта {ConstructInfo.Name}", (int)(City._Time._WorldTime / 60), ConstructInfo.WoodCost / 2, CityStorage.ResourceType.Wood));
                City._Storage._Wood += ConstructInfo.WoodCost / 2;
            }
            if (ConstructInfo.MetalCost > 0)
            {
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"отмена проекта {ConstructInfo.Name}", (int)(City._Time._WorldTime / 60), ConstructInfo.MetalCost / 2, CityStorage.ResourceType.Metal));
                City._Storage._Metal += ConstructInfo.MetalCost / 2;
            }
            if (ConstructInfo.BerezenuimCost > 0)
            {
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"отмена проекта {ConstructInfo.Name}", (int)(City._Time._WorldTime / 60), ConstructInfo.BerezenuimCost / 2, CityStorage.ResourceType.Berezenium));
                City._Storage._Berezenium += ConstructInfo.BerezenuimCost / 2;
            }

            Destroy(gameObject);
            City._Constructor.SpawnDemolishEffect(transform.position);
        }
    }

    private IEnumerator DestroyInside()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        foreach (Collider collider in Physics.OverlapBox(transform.position, new Vector3(ConstructInfo.Sizes[0] / 2 + 2, 50, ConstructInfo.Sizes[1] / 2 + 2), transform.rotation, TreeMask))
        {
            Destroy(collider.gameObject);
        }
    }
}
