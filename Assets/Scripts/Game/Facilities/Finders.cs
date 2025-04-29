using System.Collections.Generic;
using UnityEngine;

public class Finders : Facility
{
    [SerializeField] private float FindWork;
    private CitySally.TileContent CurrentContent = null;

    public override bool _CanBeHeated => true;

    public float _FindWork => FindWork;

    public override string _SaveInfo
    {
        get => base._SaveInfo + $"Work({FindWork})Content({(CurrentContent == null ? "-1" : StaticTools.IndexOf(City._CitySally._Contents, CurrentContent))})";
        set
        {
            base._SaveInfo = value;
        }
    }

    public override int _ColdEndurance => base._ColdEndurance + (City._Research.GetResearchLevel(CityResearch.ResearchType.Travels) >= 1 ? 4 : 0);
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        CityFactors.Factor[] factors = new CityFactors.Factor[1];

        factors[0] = new CityFactors.Factor($"Электрообеспеченность", Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2), true);

        if (Bears.Length <= 0)
        {
            factors = StaticTools.ExpandMassive(factors, new CityFactors.Factor($"Отсутствуют медведи", 0, true));
        }

        return factors;
    }
    public override float _Effectivity 
    {
        get
        {
            float work = 0;
            foreach(Bear bear in Bears)
            {
                work += bear._Work;
            }

            return work * Mathf.Clamp(City._Energosystem._Effectivity, MinimalEnergyCoeffiente, 2) ;
        }
    }

    protected override void ApplySaveInfo(Dictionary<string, string> parameters)
    {
        base.ApplySaveInfo(parameters);

        FindWork = StaticTools.StringToFloat(parameters["Work"]);
        if (parameters["Content"] != "-1")
        {
            CurrentContent = City._CitySally._Contents[StaticTools.StringToInt(parameters["Content"])];
        }
        else
        {
            CurrentContent = Closest();
        }
    }

    protected override void Start()
    {
        base.Start();

        if(FindWork <= 0 || CurrentContent == null)
        {
            CurrentContent  = Closest();

            if(CurrentContent == null)
            {
                return;
            }

            FindWork = Vector2.Distance(CurrentContent.Position, CitySally.TownPoint) / 3f;
        }
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////" +
            $"\nОбъект: {ConstructInfo.Name}" +
             $"\nУровень тепла: {(_ColdEndurance - City._Weather._Cold < 0 ? $"<color=red>{_ColdEndurance - City._Weather._Cold}</color>" : _ColdEndurance - City._Weather._Cold)}" +
            $"\nМедведи: {AssignedBears.Length}/{_MaxBearCount}" +
            $"\nРаботают: {Bears.Length}/{AssignedBears.Length}" +
            $"\nЭффективность: {(int)(_Effectivity * 100)}%" +
            $"\nСовершается работа: {(int)(_Effectivity * 100 * CityTime._DaySection)} %/ч" +
            $"\nОсталось работы: {(int)(_FindWork * 100)}%" +
            $"{(CurrentContent == null ? "\n<color=red>Больше нет сигналов !</color>" : "")}" +
            $"\n//////////";
    }

    public override void HourPassed()
    {
        if (Bears.Length < 1)
        {
            return;
        }

        if(CurrentContent == null)
        {
            CurrentContent = Closest();
            if(CurrentContent == null)
            {
                return;
            }
            else
            {
                FindWork = Vector2.Distance(CurrentContent.Position, CitySally.TownPoint) / 3f;
            }
        }

        foreach (Bear bear in Bears)
        {
            bear._Tired += CityTime._DaySection * Tiring * bear._TiredCoefficient;
        }

        FindWork -= _Effectivity * CityTime._DaySection;

        if (FindWork <= 0)
        {
            CurrentContent.State = 1;
            City._CityMessenger.AddMessage(new CityMessenger.CityMessage("Медведи найдены!", $"Наши разведустройства обнаружили наших членов экипажа в точке x{CurrentContent.Position.x} y{CurrentContent.Position.y}.\nСнарядите отряд и спасите их."));

            CurrentContent = Closest();
            if(CurrentContent != null)
            {
                FindWork = Vector2.Distance(CurrentContent.Position, CitySally.TownPoint) / 3f;
            }
        }
    }
    
    private CitySally.TileContent Closest()
    {
        if(City._CitySally._Contents.Length > 0)
        {
            CitySally.TileContent[] ordered = Ordered();

            CitySally.TileContent content = null;
            float distance = -1;
            int index = -1;

            for(int i = 0; i < City._CitySally._Contents.Length; i++)
            {
                if (City._CitySally._Contents[i].State == 0 && !StaticTools.Contains(ordered, City._CitySally._Contents[i]))
                {
                    content = City._CitySally._Contents[i];
                    distance =  Vector2.Distance(CitySally.TownPoint, content.Position);
                    index = i;
                    break;
                }
            }

            if(index == -1)
            {
                return null;
            }

            for(int i = index; i < City._CitySally._Contents.Length; i++)
            {
                CitySally.TileContent tileContent = City._CitySally._Contents[i];
               
                if(distance > Vector2.Distance(CitySally.TownPoint, tileContent.Position) && tileContent.State == 0 && !StaticTools.Contains(ordered, tileContent))
                {
                    content = tileContent;
                    distance = Vector2.Distance(CitySally.TownPoint, tileContent.Position);
                }
            }

            return content;
        }
        else
        {
            return null;
        }
    }

    private CitySally.TileContent[] Ordered()
    {
        CitySally.TileContent[] contents = new CitySally.TileContent[0];
        foreach(Facility facility in City._DataBase._Facilities)
        {
            if(facility != this && facility is Finders && (facility as Finders).CurrentContent != null)
            {
                contents = StaticTools.ExpandMassive(contents, (facility as Finders).CurrentContent);
            }
        }

        return contents;
    }
}
