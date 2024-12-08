using UnityEngine;

public class CityEnergosystem : MonoBehaviour
{
    [SerializeField] private CityTime CityTime;
    [SerializeField] private CityDataBase CityData;

    [SerializeField] private float StoredEnergy;
    [SerializeField] private float EnergyCapacity;
    [SerializeField] private float Produced;
    [SerializeField] private float Consumed;
    [SerializeField] private float EffectivityCoefficient;

    public string _SaveInfo
    {
        get
        {
            return $"{StoredEnergy};{Produced};{Consumed};{EffectivityCoefficient}";
        }
        set
        {
            if(value == null || value.Length <= 0)
            {
                return;
            }

            string[] values = value.Split(";");

            StoredEnergy = StaticTools.StringToFloat(values[0]);
            Produced = StaticTools.StringToFloat(values[1]);
            Consumed = StaticTools.StringToFloat(values[2]);
            EffectivityCoefficient = StaticTools.StringToFloat(values[3]);
        }
    }
    public float _Effectivity => EffectivityCoefficient * EffectivityCoefficient;
    public float _Consumed => Consumed;
    public float _Produced => Produced;
    public float[] _Potencial
    {
        get
        {
            float[] values = new float[4];

            float honey = City._Storage._EnergyHoney;
            float berezenium = City._Storage._Berezenium;
            float wood = City._Storage._Wood;

            foreach (Facility facility in CityData._Facilities)
            {
                if (facility is EnergyProcuder)
                {
                    switch((facility as EnergyProcuder)._Resource)
                    {
                        case CityStorage.ResourceType.EnergyHoney:
                            if(honey > 0)
                            {
                                values[0] += (facility as EnergyProcuder)._BaseProduce * (facility as EnergyProcuder)._Effectivity * Mathf.Clamp01(honey / (facility as EnergyProcuder)._BaseConsume);
                                honey -= (facility as EnergyProcuder)._BaseConsume * (facility._Enabled ? 1 : 0);
                            }
                            break;
                        case CityStorage.ResourceType.Berezenium:
                            if (honey > 0)
                            {
                                values[0] += (facility as EnergyProcuder)._BaseProduce * (facility as EnergyProcuder)._Effectivity * Mathf.Clamp01(berezenium / (facility as EnergyProcuder)._BaseConsume);
                                berezenium -= (facility as EnergyProcuder)._BaseConsume * (facility._Enabled ? 1 : 0);
                            }
                            break;
                        case CityStorage.ResourceType.Wood:
                            if (honey > 0)
                            {
                                values[0] += (facility as EnergyProcuder)._BaseProduce * (facility as EnergyProcuder)._Effectivity * Mathf.Clamp01(wood / (facility as EnergyProcuder)._BaseConsume);
                                wood -= (facility as EnergyProcuder)._BaseConsume * (facility._Enabled ? 1 : 0);
                            }
                            break;
                    }
                }
                else
                {
                    values[1] += facility._EnergyConsume;
                }
            }

            if (values[0] + StoredEnergy < values[1])
            {
                values[2] = (values[0] + StoredEnergy) / values[1];
                values[3] = values[1] * values[2];
            }
            else
            {
                values[2] = 1;
                values[3] = values[1];
            }

            return values;
        }
    }
    public float _StoredEnergy
    {
        get
        {
            return StoredEnergy;
        }
        set
        {
           StoredEnergy = value;
        }
    }
    public float _EnergyCapacity => EnergyCapacity;

    private void Start()
    {
        City._DataBase.OnFacilityChanges += UpdateCapacity;
        UpdateCapacity();
    }

    public void UpdateCapacity()
    {
        EnergyCapacity = 0;
        foreach(Facility facility in City._DataBase._Facilities)
        {
            if(facility is Accumulator)
            {
                EnergyCapacity += (facility as Accumulator)._Capacity;
            }
        }
    }

    public void DayPassed()
    {
        Produced = 0;
        Consumed = 0;

        foreach (Facility facility in CityData._Facilities)
        {
            if(facility is EnergyProcuder)
            {
                switch((facility as EnergyProcuder)._Resource)
                {
                    case CityStorage.ResourceType.EnergyHoney:
                        if(City._Storage._EnergyHoney > 0)
                        {
                            Produced += (facility as EnergyProcuder)._Produce * Mathf.Clamp01(City._Storage._EnergyHoney/(facility as EnergyProcuder)._BaseConsume);
                            City._Storage._EnergyHoney -= (facility as EnergyProcuder)._BaseConsume * (facility._Enabled ? 1 : 0);
                        }
                        break;
                    case CityStorage.ResourceType.Berezenium:
                        if (City._Storage._Berezenium > 0)
                        {
                            Produced += (facility as EnergyProcuder)._Produce * Mathf.Clamp01(City._Storage._Berezenium / (facility as EnergyProcuder)._BaseConsume);
                            City._Storage._Berezenium -= (facility as EnergyProcuder)._BaseConsume;
                        }
                        break;
                    case CityStorage.ResourceType.Wood:
                        if (City._Storage._Wood > 0)
                        {
                            Produced += (facility as EnergyProcuder)._Produce * Mathf.Clamp01(City._Storage._Wood / (facility as EnergyProcuder)._BaseConsume);
                            City._Storage._Wood -= (facility as EnergyProcuder)._BaseConsume * (facility._Enabled ? 1 : 0);
                        }
                        break;
                }
            }
            else
            {
                Consumed += facility._EnergyConsume;
            }
        }

        if (Produced + StoredEnergy < Consumed)
        {
            EffectivityCoefficient = (Produced + StoredEnergy) / Consumed;
            StoredEnergy = 0;

            Consumed *= EffectivityCoefficient;
        }
        else
        {
            EffectivityCoefficient = 1;

            StoredEnergy = Mathf.Clamp( StoredEnergy + Produced - Consumed, 0, EnergyCapacity);
        }
    }
}
