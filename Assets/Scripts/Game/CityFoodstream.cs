using UnityEngine;

public class CityFoodstream : MonoBehaviour
{
    [SerializeField] private CityTime CityTime;
    [SerializeField] private CityDataBase CityData;

    [SerializeField] private float StoredFood;
    [SerializeField] private float FoodCapacity;
    [SerializeField] private float Produced;
    [SerializeField] private float Consumed;
    [SerializeField] private float SaturateCoefficient;

    public string _SaveInfo
    {
        get
        {
            return $"{StoredFood};{Produced};{Consumed};{SaturateCoefficient}";
        }
        set
        {
            if(value == null || value.Length <= 0)
            {
                return;
            }

            string[] values = value.Split(';');

            StoredFood = StaticTools.StringToFloat(values[0]);
            Produced = StaticTools.StringToFloat(values[1]);
            Consumed = StaticTools.StringToFloat(values[2]);
            SaturateCoefficient = StaticTools.StringToFloat(values[3]);
        }
    }
    public float _Saturation => SaturateCoefficient;
    public float[] _Potencial
    {
        get
        {
            float[] value = new float[4];

            foreach (Facility facility in CityData._Facilities)
            {
                if (facility is FoodProducer)
                {
                    value[0] += (facility as FoodProducer)._BaseProduce * (facility as FoodProducer).GetPotencialEffectivity();
                }
            }

            value[1] = CityData._Bears.Length * 2;

            if (value[0] + StoredFood < value[1])
            {
                value[2] = (value[0] + StoredFood) / value[1];
                value[3] = value[1] * value[2];
            }
            else
            {
                value[2] = 1;
                value[3] = value[1];
            }

            return value;
        }
    }
    public float _Consumed => Consumed;
    public float _Produced => Produced;
    public float _StoredFood
    {
        get
        {
            return StoredFood;
        }
        set
        {
            StoredFood = Mathf.Clamp(value, 0 ,_FoodCapacity);
        }
    }
    public float _FoodCapacity => FoodCapacity;

    private void Start()
    {
        City._DataBase.OnFacilityChanges += UpdateCapacity;
        UpdateCapacity();
    }

    public void UpdateCapacity()
    {
        FoodCapacity = 0;
        foreach (Facility facility in City._DataBase._Facilities)
        {
            if (facility is FoodStorage)
            {
                FoodCapacity += (facility as FoodStorage)._Capacity;
            }
        }
    }

    public void DayPassed()
    {
        Produced = 0;
        foreach(Facility facility in CityData._Facilities)
        {
            if(facility is FoodProducer)
            {
                Produced += (facility as FoodProducer)._Produce;
            }
        }

        Consumed = CityData._Bears.Length * 2;

        if (Produced + StoredFood < Consumed)
        {
            SaturateCoefficient = (Produced + StoredFood) / Consumed;
            Consumed *= SaturateCoefficient;

            StoredFood = 0;
        }
        else
        {
            SaturateCoefficient = 1;

            if(Produced - Consumed > 0)
            {
                if(StoredFood < FoodCapacity)
                {
                    StoredFood = Mathf.Clamp(StoredFood + Produced - Consumed, 0, FoodCapacity);
                }
            }
            else
            {
                StoredFood = Mathf.Max(StoredFood + Produced - Consumed, 0);
            }
        }
    }
}
